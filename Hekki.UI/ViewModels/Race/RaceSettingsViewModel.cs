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
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [Required(ErrorMessage = "Race name is required")]
        [MinLength(3, ErrorMessage = "Race name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Race name cannot exceed 100 characters")]
        [NotifyDataErrorInfo]
        private string _raceName = string.Empty;

        [ObservableProperty]
        private DateTime _raceDate = DateTime.Today;

        [ObservableProperty]
        private string? _selectedLocation;

        public ObservableCollection<string> AvailableLocations { get; } = [];

        public Action? CloseAction { get; set; }
        public bool DialogResult { get; private set; }

        private readonly INavigationService _navigationService;

        public RaceSettingsViewModel(
            INavigationService navigationService,
            string raceName = "",
            DateTime? raceDate = null,
            string? selectedLocation = null)
        {
            _navigationService = navigationService;
            _raceName = raceName;
            _raceDate = raceDate ?? DateTime.Today;
            _selectedLocation = selectedLocation;

            AvailableLocations.Add("Location 1"); //TODO: Load from service
            AvailableLocations.Add("Location 2");
            AvailableLocations.Add("Location 3");
        }

        private bool CanSave => !HasErrors && !string.IsNullOrWhiteSpace(RaceName);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            DialogResult = true;
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            CloseAction?.Invoke();
            _navigationService.NavigateToSelection();
        }

        partial void OnRaceNameChanged(string value)
        {
            ValidateProperty(value, nameof(RaceName));
        }
    }
}
