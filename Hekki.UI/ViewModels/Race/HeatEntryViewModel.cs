using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatEntryViewModel : ObservableObject
    {
        [ObservableProperty] private int _participantId;
        [ObservableProperty] private string _pilotName = string.Empty;
        [ObservableProperty] private int? _kartNumber;
        [ObservableProperty] private int? _gridPosition;
        [ObservableProperty] private int _seedOrder;
    }
}
