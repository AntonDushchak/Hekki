using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private int _heatNumber = 0;

        public ObservableCollection<string> DynamicColumns { get; } = [];
        public ObservableCollection<HeatGroupViewModel> Groups { get; } = [];
    }

    public partial class HeatGroupViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _groupNumber = 3;

        [ObservableProperty]
        private int _groupCapacity = 8;

        public ObservableCollection<HeatResultViewModel> Results { get; } = [];
    }

    public partial class HeatResultViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _position;

        [ObservableProperty]
        private string _kartNumber = string.Empty;

        [ObservableProperty]
        private string _pilotName = string.Empty;

        public ObservableCollection<string> DynamicData { get; } = [];
    }
}
