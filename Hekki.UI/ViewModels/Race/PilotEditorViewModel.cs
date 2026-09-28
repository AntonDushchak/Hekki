using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.DTOs.Pilot;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class PilotEditorViewModel : ObservableValidator
    {
        public PilotDto? Result { get; private set; }

        [ObservableProperty]
        private bool? _dialogResult;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _swsId = string.Empty;

        [ObservableProperty]
        private string _swsLink = string.Empty;

        [ObservableProperty]
        private string? _selectedTeam;

        [ObservableProperty]
        private string? _selectedLeague;

        [ObservableProperty]
        private bool _showProfilePicture;

        private readonly PilotDto _source;

        public ObservableCollection<string> AvailableTeams { get; }
        public ObservableCollection<string> AvailableLeagues { get; }

        public PilotEditorViewModel(
            AppSettings appSettings,
            PilotDto? pilot = null)
        {
            AvailableTeams = appSettings.Teams;
            AvailableLeagues = appSettings.Leagues;

            _source = pilot ?? new PilotDto();

            if (pilot is not null)
            {
                FirstName = pilot.FirstName;
                LastName = pilot.LastName;
                SwsLink = pilot.ProfileUrl ?? string.Empty;
                SelectedTeam = pilot.Team;
                SelectedLeague = pilot.League;
            }

        }

        private bool CanSave => !string.IsNullOrWhiteSpace(FirstName)
            && !string.IsNullOrWhiteSpace(LastName);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            Result = _source with
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                ProfileUrl = string.IsNullOrWhiteSpace(SwsLink) ? null : SwsLink.Trim(),
                Team = SelectedTeam,
                League = SelectedLeague
            };

            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }
    }
}