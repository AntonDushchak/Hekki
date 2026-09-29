using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.Exceptions;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using Hekki.UI.ViewModels.Race;
using Hekki.UI.ViewModels.Race.Session;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IDialogService _dialogService;
        private readonly RaceSessionHolder _sessionHolder;

        [ObservableProperty] private int _regulationId;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditRaceSettingsCommand))]
        private int? _raceId;
        [ObservableProperty] private string _raceName = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private DateTime _raceDate = DateTime.Today;
        [ObservableProperty] private bool _showFirstSettings;

        public bool IsNewRace => RaceId == null;

        public RaceSession? Session => _sessionHolder.Current;
        public HeatCommands HeatCommands { get; }
        public ParticipantCommands ParticipantCommands { get; }
        public ParticipantsSectionViewModel Participants { get; }
        private AppSettings _appSettings;

        public RaceViewModel(
            int regulationId,
            int? raceId,
            IRaceService raceService,
            IPilotService pilotService,
            IDialogService dialogService,
            AppSettings appSettings,
            RaceSessionHolder sessionHolder)
        {
            _sessionHolder = sessionHolder;
            RegulationId = regulationId;
            RaceId = raceId;
            _raceService = raceService;
            _dialogService = dialogService;

            Participants = new ParticipantsSectionViewModel(raceService, pilotService, dialogService, appSettings, sessionHolder);
            HeatCommands = new HeatCommands(raceService, dialogService, sessionHolder);
            ParticipantCommands = new ParticipantCommands(raceService, pilotService, dialogService, appSettings, sessionHolder);
            _appSettings = appSettings;
        }

        public Task InitializeAsync() => ExecuteSafeAsync(async () =>
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
            //TODO: Remove this on prod
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

            var raceDto = await _raceService.GetRaceDataAsync(RaceId!.Value)
                ?? throw new RaceNotFoundException(RaceId.Value);

            RaceUiMapper.ApplyTo(this, raceDto);
            _sessionHolder.Current = RaceSession.Create(raceDto);
            OnPropertyChanged(nameof(Session));
        });

        [RelayCommand]
        private Task AddTestDataAsync() => ExecuteSafeAsync(async () =>
        {
            if (RaceId == null) return;
            for (int i = 334; i <= 344; i++)
                Session?.AddParticipant(await _raceService.AddParticipantAsync(RaceId.Value, i));
        });

        protected override void OnDisposing()
        {
            Participants.Dispose();
            HeatCommands.Dispose();
            ParticipantCommands.Dispose();
        }

        [RelayCommand(CanExecute = nameof(CanEditRaceSettings))]
        private Task EditRaceSettingsAsync() => ExecuteSafeAsync(async () =>
        {
            var settingsViewModel = new RaceSettingsViewModel(_appSettings, RaceName, RaceDate, Location);
            if (_dialogService.ShowRaceSettings(settingsViewModel) != true) return;

            var location = settingsViewModel.SelectedLocation ?? string.Empty;
            await _raceService.UpdateRaceAsync(RaceId!.Value, settingsViewModel.RaceName, location, settingsViewModel.RaceDate);

            RaceName = settingsViewModel.RaceName;
            RaceDate = settingsViewModel.RaceDate;
            Location = location;
            Session?.ApplyRaceInfo(RaceName, Location, RaceDate);
        });

        private bool CanEditRaceSettings() => !IsNewRace;

        private void OpenRaceSettings()
        {
            var settingsViewModel = new RaceSettingsViewModel(_appSettings, RaceName, RaceDate, Location);

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
