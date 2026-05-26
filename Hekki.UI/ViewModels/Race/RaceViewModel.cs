using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.UI.Mappers;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ObservableObject
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private Regulation? _regulation;

        [ObservableProperty]
        private int _regulationId;

        [ObservableProperty]
        private int? _raceId;

        [ObservableProperty]
        private string _raceName = string.Empty;

        [ObservableProperty]
        private string _location = string.Empty;

        [ObservableProperty]
        private DateTime _raceDate = DateTime.Today;

        public bool IsNewRace => RaceId == null;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private PilotViewModel? _selectedPilot;

        public ObservableCollection<RaceParticipantViewModel> Participants { get; } = [];
        public ObservableCollection<PilotViewModel> FilteredPilots { get; } = [];
        public ObservableCollection<HeatViewModel> Heats { get; } = [];

        public RaceViewModel(
            int regulationId, 
            int? raceId, 
            IRaceService raceService,
            IPilotService pilotService)
        {
            RegulationId = regulationId;
            RaceId = raceId;
            _raceService = raceService;
            _pilotService = pilotService;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _regulation = await _raceService.GetRaceRegulationAsync(RegulationId);

            if (RaceId == null)
            {
                //TODO: New race
            }
            else
            {
                await LoadRaceAsync();
            }
        }

        private async Task CreateRaceAsync()
        {
            if (!IsNewRace)
                return;

            var newRaceId = await _raceService.CreateRaceAsync(
                RaceName, 
                Location, 
                RaceDate, 
                RegulationId);

            RaceId = newRaceId;
            await LoadRaceAsync();
        }

        private async Task LoadRaceAsync()
        {
            if (RaceId == null)
                return;

            var race = await _raceService.GetRaceByIdAsync(RaceId.Value);
            if (race == null)
                return;

            RaceName = race.Name;
            Location = race.Location;
            RaceDate = race.Date;

            // Load participants with pilot info first
            var participants = await _raceService.GetRaceParticipantsAsync(RaceId.Value);
            Participants.Clear();
            foreach (var participant in participants)
            {
                Participants.Add(new RaceParticipantViewModel
                {
                    Id = participant.Participant.Id,
                    RaceId = participant.Participant.RaceId,
                    PilotId = participant.Participant.PilotId,
                    PilotName = participant.PilotName,
                    Team = participant.Participant.Team,
                    IsActive = participant.Participant.IsActive,
                    PilotPhotoPath = participant.PilotPhotoPath,
                    PilotProfileUrl = participant.PilotProfileUrl
                });
            }

            // Load heats with entries and results
            if (_regulation != null)
            {
                var heats = await _raceService.GetRaceHeatsAsync(RaceId.Value);
                Heats.Clear();

                foreach (var heat in heats)
                {
                    var config = _regulation.Configurations[heat.ConfigurationIndex];
                    var entries = await _raceService.GetHeatEntriesAsync(heat.Id);
                    var results = await _raceService.GetHeatResultsAsync(heat.Id);

                    var heatVm = HeatUiMapper.MapToHeatViewModel(heat, config, entries, results, participants.ToList());
                    Heats.Add(heatVm);
                }
            }
        }

        private void LoadPilots()
        {
            // TODO: Load pilots for search from IPilotService.GetAllPilotsAsync()
            // This is only for pilot search/selection, not for participant management
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterPilotsForSearch(value);
        }

        [RelayCommand]
        private async Task AddParticipantAsync(PilotViewModel? pilot)
        {
            if (pilot == null || RaceId == null) return;

            var exists = await _raceService.IsParticipantInRaceAsync(RaceId.Value, pilot.Id);
            if (exists)
                return;

            var participantId = await _raceService.AddParticipantAsync(RaceId.Value, pilot.Id, string.Empty);

            Participants.Add(new RaceParticipantViewModel
            {
                Id = participantId,
                RaceId = RaceId.Value,
                PilotId = pilot.Id,
                PilotName = pilot.Name,
                Team = string.Empty,
                IsActive = true,
                PilotPhotoPath = pilot.PhotoPath,
            });

            SearchText = string.Empty;
        }

        [RelayCommand]
        private async Task RemoveParticipantAsync(RaceParticipantViewModel participant)
        {
            if (participant == null) return;

            await _raceService.RemoveParticipantAsync(participant.Id);
            Participants.Remove(participant);
        }

        private void FilterPilotsForSearch(string searchText)
        {
            FilteredPilots.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            // TODO: Load pilots from IPilotService and filter
            // var filtered = allPilots.Where(p => 
            //     p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            //
            // foreach (var pilot in filtered)
            // {
            //     FilteredPilots.Add(pilot);
            // }
        }
    }
}
