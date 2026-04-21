using System.ComponentModel;

namespace Hekki.UI.Services
{
    public interface IPaginationService : INotifyPropertyChanged
    {
        int PageCapacity { get; set; }
        int CurrentPage { get; set; }
        int TotalItems { get; }
        int TotalPages { get; }
        int StartItem { get; }
        int EndItem { get; }
        string PagingText { get; }

        void SetTotalItems(int totalItems);
        void Next();
        void Prev();
    }
}
