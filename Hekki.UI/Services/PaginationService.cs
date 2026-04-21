using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.Services
{
    public class PaginationService : ObservableObject, IPaginationService
    {
        private int _pageCapacity = 6;
        private int _currentPage = 1;
        private int _totalItems = 0;

        public int PageCapacity
        {
            get => _pageCapacity;
            set
            {
                if (SetProperty(ref _pageCapacity, value))
                {
                    if (_currentPage > TotalPages)
                        CurrentPage = TotalPages;
                    OnPaginationChanged();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                var newVal = Math.Max(1, Math.Min(value, TotalPages));
                if (SetProperty(ref _currentPage, newVal))
                {
                    OnPaginationChanged();
                }
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            private set => SetProperty(ref _totalItems, value);
        }

        public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageCapacity);

        public int StartItem => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageCapacity + 1;

        public int EndItem => TotalItems == 0 ? 0 : Math.Min(CurrentPage * PageCapacity, TotalItems);

        public string PagingText => $"Showing {StartItem}-{EndItem} of {TotalItems}";

        public void SetTotalItems(int totalItems)
        {
            TotalItems = Math.Max(0, totalItems);
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;
            OnPaginationChanged();
        }

        public void Next()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        public void Prev()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        private void OnPaginationChanged()
        {
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(StartItem));
            OnPropertyChanged(nameof(EndItem));
            OnPropertyChanged(nameof(PagingText));
        }
    }
}
