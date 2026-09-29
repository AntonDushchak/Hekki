using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race.Session
{
    public class HeatGroupViewModel
    {
        public HeatGroupViewModel(HeatViewModel heat, int groupId, int groupNumber, int groupCapacity)
        {
            Heat = heat;
            GroupId = groupId;
            GroupNumber = groupNumber;
            GroupCapacity = groupCapacity;
        }

        public HeatViewModel Heat { get; }
        public int GroupId { get; }
        public int GroupNumber { get; }
        public int GroupCapacity { get; }

        public ObservableCollection<HeatRowViewModel> Rows { get; } = [];

        internal void AddEmptySlots()
        {
            if (Rows.Any(row => row.HasParticipant)) return;

            for (var i = Rows.Count; i < GroupCapacity; i++)
                Rows.Add(new HeatRowViewModel(null));
        }
    }
}
