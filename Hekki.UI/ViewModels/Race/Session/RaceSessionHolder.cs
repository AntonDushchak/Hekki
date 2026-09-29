using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels.Race.Session
{
    public partial class RaceSessionHolder : ObservableObject
    {
        [ObservableProperty] private RaceSession? _current;
    }
}
