using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class PilotViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _pilotId;

        [ObservableProperty]
        private string _name = string.Empty;
    }
}
