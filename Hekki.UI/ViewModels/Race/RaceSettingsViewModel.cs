using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Hekki.UI.ViewModels
{
    public partial class RaceSettingsViewModel : ObservableValidator
    {
        [ObservableProperty]
        private bool? _dialogResult;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [LocalizedRequired("err_NameRequired")]
        [LocalizedMinLength(3, "err_NameTooShort")]
        [LocalizedMaxLength(100, "err_NameTooLong")]
        [NotifyDataErrorInfo]
        private string _raceName = string.Empty;

        [ObservableProperty]
        private DateTime _raceDate = DateTime.Today;

        [ObservableProperty]
        private string? _selectedLocation;
        public ObservableCollection<string> AvailableLocations { get; } = [];


        public RaceSettingsViewModel(
            AppSettings appSettings,
            string raceName = "",
            DateTime? raceDate = null,
            string? selectedLocation = null)
        {
            _raceName = raceName;
            _raceDate = raceDate ?? DateTime.Today;
            _selectedLocation = selectedLocation;

            AvailableLocations = appSettings.Locations;
        }

        private bool CanSave => !HasErrors && !string.IsNullOrWhiteSpace(RaceName);


        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }

        partial void OnRaceNameChanged(string value)
        {
            ValidateProperty(value, nameof(RaceName));
        }
    }
}
