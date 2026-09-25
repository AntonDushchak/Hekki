using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race.TotalTable
{
    public partial class TotalTableViewModel : ViewModelBase
    {
        public IReadOnlyList<RaceParticipantViewModel> Participants { get; private set; } = [];
        public IReadOnlyList<HeatViewModel> Heats { get; private set; } = [];
        public ObservableCollection<TotalTableRowViewModel> TotalTableRows { get; } = [];
        public ObservableCollection<ColumnViewModel> Columns { get; } = [];
        [ObservableProperty] private bool _isAddPilotEditorOpen = false;

        public void Initialize(IReadOnlyList<HeatViewModel> heats, IReadOnlyList<RaceParticipantViewModel> participants)
        {
            IsAddPilotEditorOpen = false;

            Heats = heats;
            Participants = participants;

            BuildColumns();
            RebuildStandings();
        }

        public void AddRow(RaceParticipantViewModel participant)
        {
            TotalTableRows.Add(BuildRow(participant));
        }

        public void RemoveRow(Guid participantId)
        {
            var row = TotalTableRows.FirstOrDefault(r => r.Participant.Id == participantId);
            if (row != null) TotalTableRows.Remove(row);
        }

        public void RefreshAssignments()
        {
            RebuildStandingsKart();
        }

        private void RebuildStandingsHeat(int heatId)
        {
            //TotalTableRows[0].Cells[0]..Clear();
            //foreach (var participant in _participants)
            //    TotalTableRows.Add(BuildRow(participant));
        }
        private void RebuildStandingsKart()
        {
            var column = Columns
                .OfType<ParticipantColumn>()
                .FirstOrDefault();

            if (column is null)
                return;

            UpdateColumn(column);
        }

        private void UpdateColumn(ColumnViewModel column)
        {
            var columnIndex = Columns.IndexOf(column);

            if (columnIndex < 0)
                return;

            foreach (var row in TotalTableRows)
            {
                var context = new ParticipantRaceContext(row.Participant, Heats);
                column.UpdateCell(row.Cells[columnIndex], context);
            }
        }

        private void RebuildStandings()
        {
            TotalTableRows.Clear();
            foreach (var participant in Participants)
                TotalTableRows.Add(BuildRow(participant));
        }

        private void BuildColumns()
        {
            Columns.Clear();

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
        private void OpenAddPilotEditor()
        {
            IsAddPilotEditorOpen = true;
        }
    }
}
