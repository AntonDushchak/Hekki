namespace Hekki.UI.ViewModels.Race.TotalTable
{
    public class TotalTableRowViewModel : TableRowViewModel
    {
        public RaceParticipantViewModel Participant { get; }

        public TotalTableRowViewModel(RaceParticipantViewModel participant)
        {
            Participant = participant;
        }
    }
}
