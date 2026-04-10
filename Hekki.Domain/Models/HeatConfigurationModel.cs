using System.Text.Json;

namespace Hekki.Domain.Models
{
    public class HeatConfigurationModel
    {
        public string Name { get; set; } = string.Empty;
        public MethodSettings Shuffle { get; set; } = new();
        public MethodSettings Grouping { get; set; } = new();
        public MethodSettings KartAssignment { get; set; } = new();
        public MethodSettings Scoring { get; set; } = new();
    }

    public class MethodSettings
    {
        public string MethodId { get; set; } = string.Empty;

        public Dictionary<string, JsonElement> Arguments { get; set; } = [];
    }
}
