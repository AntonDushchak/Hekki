using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
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
        private RegulationEditDto? _regulation;

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
            _regulation = await _raceService.GetRegulationEditAsync(RegulationId);

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
            if (RaceId == null)
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
        });

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

            await LoadRaceAsync();
        });

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
    }
}
