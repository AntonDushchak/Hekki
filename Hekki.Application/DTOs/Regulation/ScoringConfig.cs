using Hekki.Application.Methods;

namespace Hekki.Application.DTOs.Regulation
{
    public record ScoringConfig
    {
        public required IScoreAssignmentMethod Method { get; init; }
        public bool UsePenalties { get; init; }
    }
}
