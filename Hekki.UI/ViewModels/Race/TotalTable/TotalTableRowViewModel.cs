using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels.Race.TotalTable
{
    public partial class TotalTableRowViewModel : ObservableObject
    {
        public RaceParticipantViewModel Participant { get; }
        public List<CellViewModel> Cells { get; } = [];

        public TotalTableRowViewModel(RaceParticipantViewModel participant)
        {
            Participant = participant;
        }
    }
}
