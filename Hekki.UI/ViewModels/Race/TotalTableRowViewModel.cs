using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class TotalTableRowViewModel : ObservableObject
    {
        public RaceParticipantViewModel Participant { get; }
        public List<CellViewModel> Cells { get; } = [];
        [ObservableProperty] private string _kartNumbersDisplayText = string.Empty;

        public TotalTableRowViewModel(RaceParticipantViewModel participant)
        {
            Participant = participant;
        }
    }
}
