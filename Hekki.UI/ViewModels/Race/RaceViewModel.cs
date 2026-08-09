using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.UI.Mappers;
using Hekki.Application.Messages.Race;
using Hekki.UI.Services;
using Hekki.UI.ViewModels.Race;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private int _regulationId;
        [ObservableProperty] private int? _raceId;
        [ObservableProperty] private string _raceName = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private DateTime _raceDate = DateTime.Today;
        [ObservableProperty] private bool _showFirstSettings;

        public bool IsNewRace => RaceId == null;

        public ParticipantsSectionViewModel Participants { get; }
        public TotalTableViewModel TotalTable { get; }
        public HeatsTableViewModel HeatsTable { get; }

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
            _dialogService = dialogService;
            _navigationService = navigationService;

            Participants = new ParticipantsSectionViewModel(raceService, pilotService);
            TotalTable = new TotalTableViewModel();
            HeatsTable = new HeatsTableViewModel(raceService);

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

        partial void OnShowFirstSettingsChanged(bool value)
        {
            if (!value) return;
            RaceName = "Test";
            RaceDate = DateTime.UtcNow;
            Location = "Location 1";
            _ = CreateRaceAsync();

            //if (value)
            //{
            //    OpenRaceSettings();
            //}
        }

        private Task CreateRaceAsync() => ExecuteSafeAsync(async () =>
        {
            if (!IsNewRace)
                return;

            RaceId = await _raceService.CreateRaceAsync(
                RaceName,
                Location,
                RaceDate,
                RegulationId);

            await _raceService.GenerateHeatsWithGroupsAsync(RaceId.Value);
            await LoadRaceAsync();
        });

        private Task LoadRaceAsync() => ExecuteSafeAsync(async () =>
        {
            if (IsNewRace)
                return;

            var raceDto = await _raceService.GetRaceDataAsync(RaceId!.Value);
            if (raceDto == null)
                return;

            RaceUiMapper.ApplyTo(this, raceDto);

            var participantVms = raceDto.Participants
                .Select(PilotUiMapper.MapToParticipantViewModel)
                .ToList();

            var heatVms = raceDto.Heats
                .Select(HeatUiMapper.MapToHeatViewModel)
                .ToList();

            Participants.Initialize(RaceId!.Value, participantVms);
            HeatsTable.Initialize(RaceId!.Value, heatVms);
            TotalTable.Initialize(RaceId!.Value, heatVms, participantVms);
        });

        [RelayCommand]
        private Task AddTestDataAsync() => ExecuteSafeAsync(async () =>
        {
            if (RaceId == null) return;
            for (int i = 1; i <= 10; i++)
                await _raceService.AddParticipantAsync(RaceId.Value, i);
        });

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
    }
}
