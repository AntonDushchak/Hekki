using Hekki.Application.Abstrations;
using Hekki.UI.Services;
using System.ComponentModel;


namespace Hekki.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IRegulationService _regulationService;
        private readonly IViewModelFactory _viewModelFactory;

        public TopPanelViewModel TopPanelVM { get; }

        private object _currentPageVM;

        public object CurrentPageVM
        {
            get => _currentPageVM;
            set
            {
                _currentPageVM = value;
                OnPropertyChanged(nameof(CurrentPageVM));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MainViewModel(INavigationService navigationService, IRegulationService regulationService, IViewModelFactory viewModelFactory)
        {
            TopPanelVM = new TopPanelViewModel();

            _navigationService = navigationService;
            _regulationService = regulationService;
            _viewModelFactory = viewModelFactory;

            navigationService.Navigate = OnNavigate;
            NavigateToSelection();
        }

        private void NavigateToSelection()
        {
            //_navigationService.Go(_viewModelFactory.Create<CreateRaceViewModel>());
            _navigationService.Go(_viewModelFactory.Create<SelectionViewModel>());
        }

        private void OnNavigate(object vm)
        {
            CurrentPageVM = vm;

            switch (vm)
            {
                case RaceViewModel:
                    TopPanelVM.LeftTopPanelContent = new RaceTopPanelViewModel();
                    break;

                case CreateRaceViewModel:
                    TopPanelVM.LeftTopPanelContent = new CreateRaceTopPanelViewModel();
                    break;

                case SelectionViewModel:
                    TopPanelVM.LeftTopPanelContent = new SelectionTopPanelViewModel();
                    break;

                default:
                    break;
            }
        }
    }
}
