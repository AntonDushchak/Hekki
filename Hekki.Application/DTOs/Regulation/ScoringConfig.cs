using Hekki.Application.Methods;

namespace Hekki.Application.DTOs.Regulation
{
    public class ScoringConfig
    {
        public IScoreAssignmentMethod Method { get; set; }
        public bool UsePenalties { get; set; }
    }
}
