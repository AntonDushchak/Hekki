using System.Text.Json.Serialization;

namespace Hekki.Infrastructure.DTOs
{
    public class PilotJsonDto
    {
        [JsonPropertyName("Driver Name")]
        public string DriverName { get; set; } = string.Empty;

        [JsonPropertyName("Profile URL")]
        public string ProfileUrl { get; set; } = string.Empty;

        [JsonPropertyName("Photo URL")]
        public string PhotoUrl { get; set; } = string.Empty;

        [JsonPropertyName("Team/Track")]
        public string? TeamTrack { get; set; }

        [JsonPropertyName("Country")]
        public string? Country { get; set; }
    }
}
