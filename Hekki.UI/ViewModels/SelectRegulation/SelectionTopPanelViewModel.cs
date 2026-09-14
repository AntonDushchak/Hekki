using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public partial class TopPanelViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly IAppSettingsService _appSettingsService;

        [ObservableProperty]
        private string _title = string.Empty;

        public RegulationPickerViewModel RegulationPicker { get; }

        public TopPanelViewModel(
            RegulationPickerViewModel regulationPicker,
            IDialogService dialogService,
            IAppSettingsService appSettingsService)
        {
            RegulationPicker = regulationPicker;
            _dialogService = dialogService;
            _appSettingsService = appSettingsService;
        }

        [RelayCommand]
        private void OpenSettings()
        {
            var viewModel = new MainSettingsViewModel(_appSettingsService);
            _dialogService.ShowMainSettings(viewModel);
        }
    }
}
