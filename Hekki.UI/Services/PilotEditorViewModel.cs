using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.DTOs.Pilot;
using System.Collections.ObjectModel;

namespace Hekki.UI.Services
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

        private int _pilotId;

        public ObservableCollection<string> AvailableTeams { get; } = [];
        public ObservableCollection<string> AvailableLeagues { get; } = [];

        public PilotEditorViewModel(
            PilotDto? pilot = null,
            IEnumerable<string>? teams = null,
            IEnumerable<string>? leagues = null)
        {
            if (pilot is not null)
            {
                _pilotId = pilot.Id;
                FirstName = pilot.FirstName;
                LastName = pilot.LastName;
                SwsLink = pilot.ProfileUrl ?? string.Empty;
                SelectedTeam = pilot.Team;
                SelectedLeague = pilot.League;
            }

            if (teams is not null)
            {
                foreach (var team in teams)
                    AvailableTeams.Add(team);
            }

            if (leagues is not null)
            {
                foreach (var league in leagues)
                    AvailableLeagues.Add(league);
            }
        }

        private bool CanSave => !string.IsNullOrWhiteSpace(FirstName)
            && !string.IsNullOrWhiteSpace(LastName);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            Result = new PilotDto
            {
                Id = _pilotId,
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