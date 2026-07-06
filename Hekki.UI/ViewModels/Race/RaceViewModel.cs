using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ObservableObject
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private readonly INavigationService _navigationService;
        private RegulationEditDto? _regulation;

        [ObservableProperty] private int _regulationId;

        [ObservableProperty] private int? _raceId;

        [ObservableProperty] private string _raceName = string.Empty;

        [ObservableProperty] private string _location = string.Empty;

        [ObservableProperty] private DateTime _raceDate = DateTime.Today;
        [ObservableProperty] private RaceDataDto _raceData;

        public bool IsNewRace => RaceId == null;

        [ObservableProperty] private string _searchText = string.Empty;

        [ObservableProperty] private PilotViewModel? _selectedPilot;
        [ObservableProperty] private bool _isPopupOpen;
        [ObservableProperty] private bool _showFirstSettings;

        private bool _isUpdatingFromSelection;
        private CancellationTokenSource? _searchCancellation;

        public ObservableCollection<RaceParticipantViewModel> Participants { get; } = [];
        public ObservableCollection<PilotViewModel> FilteredPilots { get; } = [];
        public ObservableCollection<HeatViewModel> Heats { get; } = [];

        public RaceViewModel(
            int regulationId,
            int? raceId,
            IRaceService raceService,
            IPilotService pilotService,
            INavigationService navigationService)
        {
            RegulationId = regulationId;
            RaceId = raceId;
            _raceService = raceService;
            _pilotService = pilotService;
            _navigationService = navigationService;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _regulation = await _raceService.GetRegulationEditAsync(RegulationId);

            if (RaceId == null)
            {
                ShowFirstSettings = true;
            }
            else
            {
                await LoadRaceAsync();
            }
        }

        private async Task LoadRaceAsync()
        {
            if (RaceId == null)
                return;

            var race = await _raceService.GetRaceDataAsync(RaceId.Value);
            if (race == null)
                return;

            RaceData = race;
            RaceName = race.RaceName;
            Location = race.Location;
            RaceDate = race.Date;

            // Load participants with pilot info first
            if (RaceData.Participants != null)
            {
                var participants = RaceData.Participants;
                Participants.Clear();
                foreach (var pilot in participants)
                {
                    Participants.Add(new RaceParticipantViewModel
                    {
                        Id = pilot.ParticipantId,
                        RaceId = RaceId.Value,
                        PilotId = pilot.PilotId,
                        PilotName = pilot.Name,
                        Team = pilot.Team,
                        IsActive = true,
                        PilotPhotoPath = pilot.PhotoPath,
                    });
                }
            }

            // Load heats with groups and results
            if (RaceData.Heats != null)
            {
                var heats = RaceData.Heats;
                Heats.Clear();
                foreach (var heat in heats)
                {
                    var heatVm = HeatUiMapper.MapToHeatViewModel(heat);
                    Heats.Add(heatVm);
                }
            }
        }

        partial void OnSelectedPilotChanged(PilotViewModel? value)
        {
            if (value == null) return;

            _isUpdatingFromSelection = true;

            SearchText = value.Name;

            IsPopupOpen = false;

            _isUpdatingFromSelection = false;
        }

        partial void OnShowFirstSettingsChanged(bool value)
        {
            if (value)
            {
                OpenRaceSettings();
            }
        }

        private void OpenRaceSettings()
        {
            var settingsViewModel = new RaceSettingsViewModel(
                _navigationService,
                RaceName,
                RaceDate,
                Location);

            var settingsWindow = new Views.RaceSettingsWindow
            {
                DataContext = settingsViewModel
            };

            if (settingsWindow.ShowDialog() == true)
            {
                RaceName = settingsViewModel.RaceName;
                RaceDate = settingsViewModel.RaceDate;
                Location = settingsViewModel.SelectedLocation ?? string.Empty;

                _ = CreateRaceAsync();
            }

            ShowFirstSettings = false;
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

        partial void OnSearchTextChanged(string value)
        {
            if (_isUpdatingFromSelection) return;

            if (string.IsNullOrWhiteSpace(value))
            {
                FilteredPilots.Clear();
                IsPopupOpen = false;
                return;
            }

            if (SelectedPilot != null && SelectedPilot.Name != value)
            {
                SelectedPilot = null;
            }

            _ = FilterPilotsForSearchAsync(value);
        }

        private async Task FilterPilotsForSearchAsync(string searchText)
        {
            _searchCancellation?.Cancel();
            _searchCancellation = new CancellationTokenSource();
            var ct = _searchCancellation.Token;

            if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
            {
                FilteredPilots.Clear();
                IsPopupOpen = false;
                return;
            }

            try
            {
                await Task.Delay(300, ct);

                var pilots = await _pilotService.SearchPilotsByNameAsync(searchText, ct);

                if (ct.IsCancellationRequested)
                    return;

                FilteredPilots.Clear();
                foreach (var pilot in pilots)
                {
                    FilteredPilots.Add(new PilotViewModel
                    {
                        PilotId = pilot.Id,
                        Name = pilot.Name,
                    });
                }

                IsPopupOpen = FilteredPilots.Count > 0;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Search canceled: user continues typing.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching pilots: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddParticipantAsync(PilotViewModel? pilot)
        {
            if (pilot == null || RaceId == null) return;

            var exists = RaceData.Participants.Any(p => p.PilotId == pilot.PilotId);
            if (exists)
                return;

            var participant = await _raceService.AddParticipantAsync(RaceId.Value, pilot.PilotId);

            Participants.Add(PilotUiMapper.MapToParticipantViewModel(participant));

            SearchText = string.Empty;
        }

        [RelayCommand]
        private async Task RemoveParticipantAsync(RaceParticipantViewModel participant)
        {
            if (participant == null) return;

            await _raceService.RemoveParticipantAsync(participant.Id);
            Participants.Remove(participant);
            //RaceData.Participants.RemoveAll(p => p.PilotId == participant.PilotId);
        }
    }
}
