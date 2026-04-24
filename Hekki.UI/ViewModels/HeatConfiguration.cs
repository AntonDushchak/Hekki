using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;

namespace Hekki.UI.ViewModels
{
    public partial class HeatConfiguration : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        public MethodSettingsVm Shuffle { get; set; } = new();
        public MethodSettingsVm Grouping { get; set; } = new();
        public MethodSettingsVm KartAssignment { get; set; } = new();
        public MethodSettingsVm Scoring { get; set; } = new();
        public int NumberOfGroups { get; set; }
        public int GroupCapacity { get; set; }
    }

    public partial class MethodSettingsVm : ObservableObject
    {
        [ObservableProperty]
        private string _methodId = string.Empty;

        public Dictionary<string, JsonElement> Arguments { get; set; } = [];
    }
}
