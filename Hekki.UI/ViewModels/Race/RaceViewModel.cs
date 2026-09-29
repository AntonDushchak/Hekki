using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstractions;
using Hekki.Application.Exceptions;
using Hekki.Application.Messages;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using Hekki.UI.ViewModels.Race;
using Hekki.UI.ViewModels.Race.Session;
using Hekki.UI.ViewModels.Race.TotalTable;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceViewModel : ViewModelBase,
        IRecipient<ParticipantAddedMessage>,
        IRecipient<ParticipantUpdatedMessage>,
        IRecipient<ParticipantRemovedMessage>,
        IRecipient<ParticipantsReorderedMessage>,
        IRecipient<HeatResultChangedMessage>,
        IRecipient<GroupsAssignedMessage>,
        IRecipient<HeatAssignmentClearedMessage>
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
        private List<HeatViewModel> _heatList;
        private readonly ObservableCollection<RaceParticipantViewModel> _participantList = [];

        public bool IsNewRace => RaceId == null;

        public RaceSession? Session => _sessionHolder.Current;
        public ParticipantsSectionViewModel Participants { get; }
        public TotalTableViewModel TotalTable { get; }
        public HeatsTableViewModel HeatsTable { get; }
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

            Participants = new ParticipantsSectionViewModel(raceService, pilotService, dialogService, appSettings);
            TotalTable = new TotalTableViewModel(raceService, pilotService, dialogService, appSettings);
            HeatsTable = new HeatsTableViewModel(raceService, dialogService);
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
            _sessionHolder.Current = RaceSession.Create(raceDto);
            OnPropertyChanged(nameof(Session));

            _participantList.Clear();
            foreach (var participant in raceDto.Participants)
                _participantList.Add(PilotUiMapper.MapToParticipantViewModel(participant));

            _heatList = raceDto.Heats
                .Select(HeatUiMapper.MapToHeatViewModel)
                .ToList();

            Participants.Initialize(RaceId!.Value, _participantList);
            HeatsTable.Initialize(RaceId!.Value, _heatList);
            TotalTable.Initialize(RaceId!.Value, _heatList, _participantList);
        });

        public void Receive(ParticipantAddedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.AddParticipant(message.RaceParticipant);

            var participant = PilotUiMapper.MapToParticipantViewModel(message.RaceParticipant);
            _participantList.Add(participant);
            TotalTable.AddRow(participant);
            TotalTable.IsAddPilotEditorOpen = false;
        }

        public void Receive(ParticipantRemovedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.RemoveParticipant(message.ParticipantId);

            var participant = _participantList.FirstOrDefault(p => p.Id == message.ParticipantId);
            if (participant != null) _participantList.Remove(participant);
            TotalTable.RemoveRow(message.ParticipantId);

            foreach (var group in _heatList.SelectMany(h => h.Groups))
            {
                var rows = group.Rows.Where(r => r.Entry?.ParticipantId == message.ParticipantId).ToList();
                if (rows.Count == 0) continue;

                foreach (var row in rows)
                    group.Rows.Remove(row);
                group.RefreshCells();
            }
        }

        public void Receive(ParticipantUpdatedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.UpdateParticipant(message.RaceParticipant);

            var participantId = message.RaceParticipant.ParticipantId;
            var participant = _participantList.FirstOrDefault(p => p.Id == participantId);
            if (participant == null) return;

            PilotUiMapper.ApplyToParticipantViewModel(message.RaceParticipant, participant);

            foreach (var group in _heatList.SelectMany(h => h.Groups))
            {
                var rows = group.Rows.Where(r => r.Entry?.ParticipantId == participantId).ToList();
                if (rows.Count == 0) continue;

                foreach (var row in rows)
                    row.Entry!.PilotName = participant.FullName;
                group.RefreshCells();
            }

            TotalTable.RefreshRow(participantId);
        }

        public void Receive(ParticipantsReorderedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.ApplyOrder(message.ParticipantIds);
            _participantList.ReorderBy(message.ParticipantIds, p => p.Id);
            TotalTable.ApplyOrder(message.ParticipantIds);
        }

        public void Receive(HeatResultChangedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.SetResult(message.HeatId, message.Result);

            var participantId = message.Result.ParticipantId;
            var group = _heatList
                .Where(h => h.HeatId == message.HeatId)
                .SelectMany(h => h.Groups)
                .FirstOrDefault(g => g.Rows.Any(r => r.Entry?.ParticipantId == participantId));
            if (group == null) return;

            var row = group.Rows.First(r => r.Entry?.ParticipantId == participantId);
            row.Result = HeatUiMapper.MapToResultViewModel(message.Result);
            group.RefreshCells();
            TotalTable.RefreshRow(participantId);
        }

        public void Receive(GroupsAssignedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.ApplyAssignment(message.HeatId, message.Result);
            HeatAssignmentApplier.Apply(_heatList, message.Result);
            TotalTable.RefreshAssignments();
            HeatsTable.NotifyDrawStateChanged();
        }

        public void Receive(HeatAssignmentClearedMessage message)
        {
            if (message.RaceId != RaceId) return;

            Session?.ClearAssignment(message.HeatId);

            var heat = _heatList.FirstOrDefault(h => h.HeatId == message.HeatId);
            if (heat == null) return;

            foreach (var group in heat.Groups)
                group.ClearAssignment();

            TotalTable.RefreshAllRows();
            HeatsTable.NotifyDrawStateChanged();
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
