using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatConfiguration : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        public MethodSettings Shuffle { get; set; } = new();
        public MethodSettings Grouping { get; set; } = new();
        public MethodSettings KartAssignment { get; set; } = new();
        public MethodSettings Scoring { get; set; } = new();
    }

    public partial class MethodSettings : ObservableObject
    {
        [ObservableProperty]
        private string _methodId;

        public Dictionary<string, object> Arguments { get; set; } = [];
    }
}
