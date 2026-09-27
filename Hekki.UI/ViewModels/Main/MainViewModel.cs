using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public TopPanelViewModel TopPanelVM { get; }
        [ObservableProperty] private object? _currentPageVM = null;


        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IAppSettingsService appSettingsService)
        {
            TopPanelVM = new TopPanelViewModel(dialogService, appSettingsService);

            _navigationService = navigationService;

            _navigationService.Navigated += OnNavigate;

            //_navigationService.NavigateToRace(14); //TODO: Remove this on prod
            _navigationService.NavigateToSelection();
        }

        private void OnNavigate(object vm)
        {
            var previous = CurrentPageVM;
            CurrentPageVM = vm;
            if (!ReferenceEquals(previous, vm))
                (previous as IDisposable)?.Dispose();

            TopPanelVM.Title = vm switch
            {
                RaceViewModel => new LocalizedText("m_Race"),
                CreateRegulationViewModel => new LocalizedText("m_NewRegulation"),
                SelectionViewModel => new LocalizedText("m_Regulations"),
                _ => null
            };
        }
    }
}
