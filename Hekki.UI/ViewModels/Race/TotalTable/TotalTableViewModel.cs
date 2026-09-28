using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.Exceptions;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Hekki.UI.ViewModels.Race.TotalTable
{
    public partial class TotalTableViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private readonly IDialogService _dialogService;
        private readonly AppSettings _appSettings;
        private int? _raceId;

        public IReadOnlyList<RaceParticipantViewModel> Participants { get; private set; } = [];
        public IReadOnlyList<HeatViewModel> Heats { get; private set; } = [];
        public ObservableCollection<TotalTableRowViewModel> TotalTableRows { get; } = [];
        public ObservableCollection<TotalTableColumn> Columns { get; } = [];
        [ObservableProperty] private bool _isAddPilotEditorOpen = false;

        public TotalTableViewModel(
            IRaceService raceService,
            IPilotService pilotService,
            IDialogService dialogService,
            AppSettings appSettings)
        {
            _raceService = raceService;
            _pilotService = pilotService;
            _dialogService = dialogService;
            _appSettings = appSettings;
        }

        public void Initialize(int raceId, IReadOnlyList<HeatViewModel> heats, IReadOnlyList<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;
            IsAddPilotEditorOpen = false;

            Heats = heats;
            Participants = participants;

            BuildColumns();
            BuildRows();
        }

        public void AddRow(RaceParticipantViewModel participant)
        {
            TotalTableRows.Add(BuildRow(participant));
            RefreshRowNumbers();
        }

        public void RemoveRow(Guid participantId)
        {
            var row = TotalTableRows.FirstOrDefault(r => r.Participant.Id == participantId);
            if (row != null) TotalTableRows.Remove(row);
            RefreshRowNumbers();
        }

        public void RefreshRow(Guid participantId)
        {
            var row = TotalTableRows.FirstOrDefault(r => r.Participant.Id == participantId);
            if (row == null) return;

            var context = new ParticipantRaceContext(row.Participant, Heats);
            for (var i = 0; i < Columns.Count; i++)
                Columns[i].UpdateCell(row.Cells[i], context);
        }

        public void RefreshAllRows()
        {
            foreach (var row in TotalTableRows)
                RefreshRow(row.Participant.Id);
        }

        public void RefreshAssignments()
        {
            RefreshColumn(Columns.OfType<ParticipantColumn>().FirstOrDefault());
        }

        public Task MoveRowAsync(TotalTableRowViewModel row, int newIndex) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var oldIndex = TotalTableRows.IndexOf(row);
            if (oldIndex < 0 || oldIndex == newIndex || newIndex < 0 || newIndex >= TotalTableRows.Count) return;

            var orderedIds = TotalTableRows.Select(r => r.Participant.Id).ToList();
            orderedIds.RemoveAt(oldIndex);
            orderedIds.Insert(newIndex, row.Participant.Id);

            await _raceService.ReorderParticipantsAsync(_raceId.Value, orderedIds);
        });

        [RelayCommand]
        private Task SortByColumnAsync(TotalTableColumn column) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var orderedIds = TotalTableRows
                .Select(row => (row.Participant.Id, Key: column.GetSortKey(new ParticipantRaceContext(row.Participant, Heats))))
                .OrderBy(item => item.Key, Comparer<IComparable?>.Create(CompareSortKeys))
                .Select(item => item.Id)
                .ToList();

            await _raceService.ReorderParticipantsAsync(_raceId.Value, orderedIds);
        });

        [RelayCommand]
        private Task ReverseOrderAsync() => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var orderedIds = TotalTableRows.Select(row => row.Participant.Id).Reverse().ToList();
            await _raceService.ReorderParticipantsAsync(_raceId.Value, orderedIds);
        });

        private static int CompareSortKeys(IComparable? left, IComparable? right)
        {
            var leftEmpty = left is null || left is string { Length: 0 };
            var rightEmpty = right is null || right is string { Length: 0 };

            if (leftEmpty || rightEmpty)
                return leftEmpty.CompareTo(rightEmpty);

            if (left is string leftText && right is string rightText)
                return string.Compare(leftText, rightText, StringComparison.CurrentCultureIgnoreCase);

            return left!.CompareTo(right);
        }

        public void ApplyOrder(IReadOnlyList<Guid> participantIds)
        {
            TotalTableRows.ReorderBy(participantIds, r => r.Participant.Id);
            RefreshRowNumbers();
        }

        private void RebuildStandingsHeat(int heatId)
        {
            //TotalTableRows[0].Cells[0]..Clear();
            //foreach (var participant in _participants)
            //    TotalTableRows.Add(BuildRow(participant));
        }

        private void RefreshColumn(TotalTableColumn? column)
        {
            if (column is null)
                return;

            var columnIndex = Columns.IndexOf(column);

            if (columnIndex < 0)
                return;

            foreach (var row in TotalTableRows)
            {
                var context = new ParticipantRaceContext(row.Participant, Heats);
                column.UpdateCell(row.Cells[columnIndex], context);
            }
        }

        private void BuildRows()
        {
            TotalTableRows.Clear();
            foreach (var participant in Participants)
                TotalTableRows.Add(BuildRow(participant));
            RefreshRowNumbers();
        }

        private void RefreshRowNumbers()
        {
            var column = Columns.OfType<RowNumberColumn>().FirstOrDefault();
            if (column is null) return;

            var columnIndex = Columns.IndexOf(column);
            for (var i = 0; i < TotalTableRows.Count; i++)
                TotalTableRows[i].Cells[columnIndex].SetValue(new TextCellValue((i + 1).ToString()));
        }

        private void BuildColumns()
        {
            Columns.Clear();

            Columns.Add(new RowNumberColumn());
            Columns.Add(new LeagueColumn());
            //Columns.Add(new TeamColumn());
            //Columns.Add(new PhotoColumn());
            Columns.Add(new ParticipantColumn());

            foreach (var heat in Heats)
            {
                if (heat.ShowScore)
                    Columns.Add(new HeatScoreColumn(heat));

                if (heat.ShowTime)
                    Columns.Add(new HeatTimeColumn(heat));
            }

            if (Heats.Any(h => h.ShowTime))
                Columns.Add(new TotalTimeColumn());

            if (Heats.Any(h => h.ShowScore))
                Columns.Add(new TotalScoreColumn());

        }

        private TotalTableRowViewModel BuildRow(RaceParticipantViewModel participant)
        {
            var row = new TotalTableRowViewModel(participant);

            var context = new ParticipantRaceContext(participant, Heats);

            foreach (var column in Columns)
            {
                row.Cells.Add(column.CreateCell(context));
            }

            return row;
        }

        [RelayCommand]
        private Task EditPilotAsync(TotalTableRowViewModel row) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var pilot = await _pilotService.GetPilotByIdAsync(row.Participant.PilotId)
                ?? throw new PilotNotFoundException(row.Participant.PilotId);

            var editedPilot = _dialogService.ShowPilotEditor(new PilotEditorViewModel(_appSettings, pilot));
            if (editedPilot == null) return;

            await _raceService.UpdateParticipantPilotAsync(_raceId.Value, row.Participant.Id, editedPilot);
        });

        [RelayCommand]
        private Task DeleteParticipantAsync(TotalTableRowViewModel row) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var confirmed = _dialogService.Confirm(
                Localizer.Get("m_DeleteParticipantTitle"),
                Localizer.Get("m_DeleteParticipantConfirm", row.Participant.FullName),
                Localizer.Get("m_Delete"));
            if (!confirmed) return;

            await _raceService.RemoveParticipantAsync(_raceId.Value, row.Participant.Id);
        });

        [RelayCommand]
        private void ToggleParticipantActive(TotalTableRowViewModel row)
        {
            // TODO: exclude/include the participant from the race
        }

        [RelayCommand(CanExecute = nameof(HasProfileLink))]
        private void OpenProfileLink(TotalTableRowViewModel row)
        {
            if (TryGetProfileLink(row, out var link))
                Process.Start(new ProcessStartInfo(link.AbsoluteUri) { UseShellExecute = true });
        }

        private static bool HasProfileLink(TotalTableRowViewModel? row) => TryGetProfileLink(row, out _);

        private static bool TryGetProfileLink(TotalTableRowViewModel? row, out Uri link)
        {
            link = null!;
            return Uri.TryCreate(row?.Participant.PilotProfileUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && (link = uri) != null;
        }

        [RelayCommand]
        private void OpenAddPilotEditor()
        {
            IsAddPilotEditorOpen = true;
        }
    }
}
