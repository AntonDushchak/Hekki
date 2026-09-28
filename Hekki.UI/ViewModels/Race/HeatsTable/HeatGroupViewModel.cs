using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatGroupViewModel : ObservableObject
    {
        public readonly int GroupId;
        public HeatViewModel Heat { get; }

        public HeatGroupViewModel(int groupId, HeatViewModel heat)
        {
            GroupId = groupId;
            Heat = heat;
        }

        [ObservableProperty]
        private int _groupNumber = 3;

        [ObservableProperty]
        private int _groupCapacity = 8;

        public ObservableCollection<HeatRowViewModel> Rows { get; set; } = [];
        public ObservableCollection<HeatGroupColumn> Columns { get; } = [];

        public void AddEmptySlots()
        {
            if (Rows.Any(row => row.HasParticipant)) return;

            for (var i = Rows.Count; i < GroupCapacity; i++)
                Rows.Add(new HeatRowViewModel { Entry = new HeatEntryViewModel(), Result = new HeatResultViewModel() });
        }

        public void ClearAssignment()
        {
            Rows.Clear();
            AddEmptySlots();
            RefreshCells();
        }

        public void RefreshCells()
        {
            if (Columns.Count == 0)
                BuildColumns();

            foreach (var row in Rows)
            {
                if (row.Cells.Count != Columns.Count)
                {
                    row.Cells.Clear();
                    foreach (var column in Columns)
                        row.Cells.Add(column.CreateCell(row));
                    continue;
                }

                for (var i = 0; i < Columns.Count; i++)
                    Columns[i].UpdateCell(row.Cells[i], row);
            }
        }

        private void BuildColumns()
        {
            Columns.Add(new GridPositionColumn());
            Columns.Add(new KartColumn());
            Columns.Add(new PilotColumn());
            //Columns.Add(new FinishPositionColumn());

            if (Heat.ShowTime)
                Columns.Add(new BestLapColumn());

            if (Heat.ShowScore)
                Columns.Add(new ScoreColumn());

            if (Heat.ShowPenalty)
                Columns.Add(new PenaltyColumn());
        }
    }
}
