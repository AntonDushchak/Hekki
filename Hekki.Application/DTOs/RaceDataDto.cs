namespace Hekki.Application.DTOs
{
    /// <summary>
    /// DTO for race data with heats and participants
    /// </summary>
    public class RaceDataDto
    {
        public int RaceId { get; set; }
        public string RaceName { get; set; } = string.Empty;
        public List<PilotDto> Participants { get; set; } = [];
        public List<HeatDto> Heats { get; set; } = [];
    }

    /// <summary>
    /// DTO for pilot with race-specific data
    /// </summary>
    public class PilotDto
    {
        public int PilotId { get; set; }
        public int ParticipantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Team { get; set; }
        public List<string> KartNumbers { get; set; } = [];
        public Dictionary<string, string> Statistics { get; set; } = [];
    }

    /// <summary>
    /// DTO for heat with results
    /// </summary>
    public class HeatDto
    {
        public int HeatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupNumber { get; set; }
        public int HeatNumber { get; set; }
        public List<string> DynamicColumns { get; set; } = [];
        public List<HeatResultDto> Results { get; set; } = [];
    }

    /// <summary>
    /// DTO for heat result row
    /// </summary>
    public class HeatResultDto
    {
        public int Position { get; set; }
        public string KartNumber { get; set; } = string.Empty;
        public string PilotName { get; set; } = string.Empty;
        public Dictionary<string, string> DynamicData { get; set; } = [];
    }
}
