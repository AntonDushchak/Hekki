using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private int _regulationId;

        [ObservableProperty] private int? _raceId;

        [ObservableProperty] private string _raceName = string.Empty;

        [ObservableProperty] private string _location = string.Empty;

        [ObservableProperty] private DateTime _raceDate = DateTime.Today;

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
        public ObservableCollection<TotalTableRowViewModel> TotalTableRows { get; } = [];

        public RaceViewModel(
            int regulationId,
            int? raceId,
            IRaceService raceService,
            IPilotService pilotService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            RegulationId = regulationId;
            RaceId = raceId;
            _raceService = raceService;
            _pilotService = pilotService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _ = InitializeAsync();
        }

        private Task InitializeAsync() => ExecuteSafeAsync(async () =>
        {
            if (RaceId == null)
            {
                ShowFirstSettings = true;
            }
            else
            {
                await LoadRaceAsync();
            }
        });

        private Task LoadRaceAsync() => ExecuteSafeAsync(async () =>
        {
            if (IsNewRace)
                return;

            var raceDto = await _raceService.GetRaceDataAsync(RaceId.Value);
            if (raceDto == null)
                return;

            RaceUiMapper.ApplyTo(this, raceDto);

            Participants.Clear();
            foreach (var p in raceDto.Participants)
                Participants.Add(PilotUiMapper.MapToParticipantViewModel(p));

            Heats.Clear();
            foreach (var h in raceDto.Heats)
                Heats.Add(HeatUiMapper.MapToHeatViewModel(h));

            RebuildStandings();
        });

        partial void OnShowFirstSettingsChanged(bool value)
        {
            RaceName = "Test";
            RaceDate = DateTime.UtcNow;
            Location = "Location 1";
            _ = CreateRaceAsync();

            //if (value)
            //{
            //    OpenRaceSettings();
            //}
        }

        private void OpenRaceSettings()
        {
            var settingsViewModel = new RaceSettingsViewModel(
                _navigationService,
                RaceName,
                RaceDate,
                Location);

            var wasShown = _dialogService.ShowRaceSettings(settingsViewModel);

            if (wasShown == true)
            {
                RaceName = settingsViewModel.RaceName;
                RaceDate = settingsViewModel.RaceDate;
                Location = settingsViewModel.SelectedLocation ?? string.Empty;

                _ = CreateRaceAsync();
            }

            ShowFirstSettings = false;
        }

        private Task CreateRaceAsync() => ExecuteSafeAsync(async () =>
        {
            if (!IsNewRace)
                return;

            var newRaceId = await _raceService.CreateRaceAsync(
                RaceName,
                Location,
                RaceDate,
                RegulationId);

            RaceId = newRaceId;

            var heats = await _raceService.GenerateHeatsWithGroupsAsync(RaceId.Value);

            await LoadRaceAsync();
        });

        private void RebuildStandings()
        {
            TotalTableRows.Clear();

            foreach (var participant in Participants)
            {
                var row = new TotalTableRowViewModel(participant);
                var karts = new List<int>();

                foreach (var heat in Heats)
                {
                    var rowInHeat = heat.Groups.SelectMany(g => g.Rows)
                        .FirstOrDefault(r => r.Entry.ParticipantId == participant.PilotId);

                    row.HeatCells.Add(new HeatResultCellViewModel(heat, rowInHeat?.Result));

                    if (rowInHeat?.Entry.KartNumber is int kart)
                        karts.Add(kart);
                }

                row.KartNumbersDisplayText = string.Join(", ", karts);
                TotalTableRows.Add(row);
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

        private Task FilterPilotsForSearchAsync(string searchText) => ExecuteSafeAsync(async () =>
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
            catch (OperationCanceledException)
            {

            }
        });

        [RelayCommand]
        private Task AddTestDataAsync() => ExecuteSafeAsync(async () =>
        {
            await _raceService.AddParticipantAsync(RaceId.Value, 1);
            await _raceService.AddParticipantAsync(RaceId.Value, 2);
            await _raceService.AddParticipantAsync(RaceId.Value, 3);
            await _raceService.AddParticipantAsync(RaceId.Value, 4);
            await _raceService.AddParticipantAsync(RaceId.Value, 5);
            await _raceService.AddParticipantAsync(RaceId.Value, 6);
            await _raceService.AddParticipantAsync(RaceId.Value, 7);
            await _raceService.AddParticipantAsync(RaceId.Value, 8);
            await _raceService.AddParticipantAsync(RaceId.Value, 9);
            await _raceService.AddParticipantAsync(RaceId.Value, 10);

            LoadRaceAsync();
            RebuildStandings();
        });

        [RelayCommand]
        private Task AddParticipantAsync() => ExecuteSafeAsync(async () =>
        {
            var pilot = SelectedPilot;
            if (pilot == null || RaceId == null) return;

            var exists = Participants.Any(p => p.PilotId == pilot.PilotId);
            if (exists)
                return;

            var participant = await _raceService.AddParticipantAsync(RaceId.Value, pilot.PilotId);

            Participants.Add(PilotUiMapper.MapToParticipantViewModel(participant));

            SearchText = string.Empty;
        });

        [RelayCommand]
        private Task RemoveParticipantAsync(RaceParticipantViewModel participant) => ExecuteSafeAsync(async () =>
        {
            if (participant == null) return;

            await _raceService.RemoveParticipantAsync(participant.Id);
            Participants.Remove(participant);
        });

        [RelayCommand]
        private Task EditHeatAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null) return;

            // TODO: Implement heat editing logic
            // Example: Open dialog for editing heat properties
            await Task.CompletedTask;
        });

        [RelayCommand]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null) return;

            var results = await _raceService.AssignGroupsAndNumbersAsync(RaceId.Value, heat.HeatNumber);

            await UpdateParticipantsAssignmentsAsync(results.ToList());
        });

        [RelayCommand]
        private Task UpdateParticipantsAssignmentsAsync(List<GroupAssignmentResultDto> results) => ExecuteSafeAsync(async () =>
        {
            if (results == null || results.Count == 0)
                return;

            foreach (var groupResult in results)
            {
                var groupDto = groupResult.Group;

                var heatVm = Heats.FirstOrDefault(h => h.Groups.FirstOrDefault(g => g.GroupId == groupDto.Id) != null);
                if (heatVm == null) continue;

                var groupVm = heatVm.Groups.FirstOrDefault(g => g.GroupId == groupDto.Id);
                if (groupVm == null) continue;

                

            RebuildStandings();
        });

        [RelayCommand]
        private Task DeleteHeatAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null) return;

            // TODO: Implement heat deletion logic with confirmation
            // Example: Show confirmation dialog, then remove heat from collection
            // var confirmed = await _dialogService.ShowConfirmationAsync("Delete heat?");
            // if (confirmed) Heats.Remove(heat);
            await Task.CompletedTask;
        });
    }
}
