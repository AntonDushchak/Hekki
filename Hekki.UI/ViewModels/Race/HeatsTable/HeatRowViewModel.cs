using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatRowViewModel : TableRowViewModel
    {
        [ObservableProperty] private HeatEntryViewModel? _entry;
        [ObservableProperty] private HeatResultViewModel? _result;
    }
}
