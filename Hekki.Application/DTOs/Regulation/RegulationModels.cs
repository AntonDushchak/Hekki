using Hekki.Application.Methods;

namespace Hekki.Application.DTOs.Regulation
{
    public class AssignmentConfig
    {
        public required IKartNummerAssignmentMethod KartMethod { get; init; }
        public required IGroupAssignmentMethod GroupMethod { get; init; }
        public required IParticipantShuffleMethod Shuffle { get; init; }
    }

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

    public record RegulationConfig
    {
        public required IReadOnlyList<HeatConfig> HeatConfigs { get; init; }
    }

    public record ScoringConfig
    {
        public required IScoreAssignmentMethod Method { get; init; }
        public bool UsePenalties { get; init; }
    }

    public enum ScoringMode
    {
        TimeBased,
        PointsBased,
        Hybrid
    }
}
