using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public partial class HeatSettingsViewModel : ViewModelBase
    {
        [ObservableProperty] private bool? _dialogResult;

        public HeatSettingsViewModel(IMethodCatalogService methodCatalog, HeatConfigurationViewModel heat)
        {
            Heat = heat;
            Editor = new MethodSettingsEditorViewModel(methodCatalog) { SelectedHeat = heat };
        }

        public HeatConfigurationViewModel Heat { get; }
        public MethodSettingsEditorViewModel Editor { get; }

        [RelayCommand]
        private void Save()
        {
            if (!Heat.IsValid())
            {
                ShowError(Localizer.Get("err_NoScoringType", Heat.Name));
                return;
            }

            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }
    }
}
