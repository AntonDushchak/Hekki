using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
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
