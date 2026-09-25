using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstractions;
using Hekki.Application.Exceptions;
using Hekki.Application.Messages;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using Hekki.UI.ViewModels.Race;
using Hekki.UI.ViewModels.Race.TotalTable;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ViewModelBase,
        IRecipient<ParticipantAddedMessage>,
        IRecipient<ParticipantRemovedMessage>,
        IRecipient<GroupsAssignedMessage>
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
        private List<HeatViewModel> _heatList;
        private readonly ObservableCollection<RaceParticipantViewModel> _participantList = [];

        public bool IsNewRace => RaceId == null;

        public ParticipantsSectionViewModel Participants { get; }
        public TotalTableViewModel TotalTable { get; }
        public HeatsTableViewModel HeatsTable { get; }
        private AppSettings _appSettings;

        public RaceViewModel(
            int regulationId,
            int? raceId,
            IRaceService raceService,
            IPilotService pilotService,
            INavigationService navigationService,
            IDialogService dialogService,
            AppSettings appSettings)
        {
            RegulationId = regulationId;
            RaceId = raceId;
            _raceService = raceService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            Participants = new ParticipantsSectionViewModel(raceService, pilotService, dialogService, appSettings);
            TotalTable = new TotalTableViewModel();
            HeatsTable = new HeatsTableViewModel(raceService);
            _heatList = new List<HeatViewModel>();
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

            _participantList.Clear();
            foreach (var participant in raceDto.Participants)
                _participantList.Add(PilotUiMapper.MapToParticipantViewModel(participant));

            _heatList = raceDto.Heats
                .Select(HeatUiMapper.MapToHeatViewModel)
                .ToList();

            Participants.Initialize(RaceId!.Value, _participantList);
            HeatsTable.Initialize(RaceId!.Value, _heatList);
            TotalTable.Initialize(_heatList, _participantList);
        });

        public void Receive(ParticipantAddedMessage message)
        {
            if (message.RaceId != RaceId) return;

            var participant = PilotUiMapper.MapToParticipantViewModel(message.RaceParticipant);
            _participantList.Add(participant);
            TotalTable.AddRow(participant);
            TotalTable.IsAddPilotEditorOpen = false;
        }

        public void Receive(ParticipantRemovedMessage message)
        {
            if (message.RaceId != RaceId) return;

            var participant = _participantList.FirstOrDefault(p => p.Id == message.ParticipantId);
            if (participant != null) _participantList.Remove(participant);
            TotalTable.RemoveRow(message.ParticipantId);
        }

        public void Receive(GroupsAssignedMessage message)
        {
            if (message.RaceId != RaceId) return;

            HeatAssignmentApplier.Apply(_heatList, message.Result);
            TotalTable.RefreshAssignments();
        }

        [RelayCommand]
        private Task AddTestDataAsync() => ExecuteSafeAsync(async () =>
        {
            if (RaceId == null) return;
            for (int i = 334; i <= 344; i++)
                await _raceService.AddParticipantAsync(RaceId.Value, i);
        });

        protected override void OnDisposing()
        {
            Participants.Dispose();
            HeatsTable.Dispose();
            TotalTable.Dispose();
        }

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
