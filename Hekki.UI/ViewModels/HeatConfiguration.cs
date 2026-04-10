using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;

namespace Hekki.UI.ViewModels
{
    public partial class HeatConfiguration : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        public MethodSettingsVm Shuffle { get; set; } = new();
        public MethodSettingsVm Grouping { get; set; } = new();
        public MethodSettingsVm KartAssignment { get; set; } = new();
        public MethodSettingsVm Scoring { get; set; } = new();
    }

    public partial class MethodSettingsVm : ObservableObject
    {
        [ObservableProperty]
        private string _methodId;

        public Dictionary<string, JsonElement> Arguments { get; set; } = [];
    }
}
