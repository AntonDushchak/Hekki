using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private const string LightTheme = "Light";
        private const string DarkTheme = "Dark";

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAppSettingsService _appSettingsService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanGoBack))]
        private object? _currentPageVM = null;

        public bool CanGoBack => CurrentPageVM is not null and not SelectionViewModel;

        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IAppSettingsService appSettingsService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _appSettingsService = appSettingsService;

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
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.NavigateToSelection();
        }

        [RelayCommand]
        private Task ToggleThemeAsync() => ExecuteSafeAsync(async () =>
        {
            var theme = _appSettingsService.Settings.Theme == DarkTheme ? LightTheme : DarkTheme;
            await _appSettingsService.SetThemeAsync(theme);
        });

        [RelayCommand]
        private void OpenSettings()
        {
            _dialogService.ShowMainSettings(new MainSettingsViewModel(_appSettingsService));
        }
    }
}
