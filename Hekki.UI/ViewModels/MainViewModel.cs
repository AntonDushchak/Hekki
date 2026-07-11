using Hekki.UI.Services;
using System.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;

        public TopPanelViewModel TopPanelVM { get; }

        private object? _currentPageVM;

        public object? CurrentPageVM
        {
            get => _currentPageVM;
            set
            {
                _currentPageVM = value;
                OnPropertyChanged(nameof(CurrentPageVM));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MainViewModel(INavigationService navigationService, RegulationPickerViewModel regulationPicker)
        {
            TopPanelVM = new TopPanelViewModel(regulationPicker);

            _navigationService = navigationService;

            _navigationService.Navigated += OnNavigate;

            _navigationService.NavigateToRace(14);
            //_navigationService.NavigateToSelection();
        }

        private void OnNavigate(object vm)
        {
            CurrentPageVM = vm;

            TopPanelVM.Title = vm switch
            {
                RaceViewModel => "Race",
                CreateRegulationViewModel => "Create",
                SelectionViewModel => "Selection",
                _ => string.Empty
            };
        }
    }
}
