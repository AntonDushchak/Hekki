namespace Hekki.Application.DTOs.Regulation
{
    public record HeatConfig
    {
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; init; }
        public int GroupCount { get; init; }
        public int ParticipantsPerGroup { get; init; }
        public ScoringMode ScoringMode { get; init; }
        public required ScoringConfig Scoring { get; init; }
        public required AssignmentConfig Assignment { get; init; }
    }
}
