using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatResultCellViewModel : ObservableObject
    {
        public HeatViewModel Heat { get; }
        public HeatResultViewModel? Result { get; }

        public HeatResultCellViewModel(HeatViewModel heat, HeatResultViewModel? result)
        {
            Heat = heat;
            Result = result;
        }
    }
}
