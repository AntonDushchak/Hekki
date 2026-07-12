using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class TotalTableRowViewModel : ObservableObject
    {
        public RaceParticipantViewModel Participant { get; }
        public ObservableCollection<HeatResultCellViewModel> HeatCells { get; } = [];
        [ObservableProperty] private string _kartNumbersDisplayText = string.Empty;

        public TotalTableRowViewModel(RaceParticipantViewModel participant)
        {
            Participant = participant;
        }
    }
}
