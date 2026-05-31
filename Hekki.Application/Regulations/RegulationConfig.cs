using Hekki.Application.Methods;

namespace Hekki.Application.Regulations
{
    public class RegulationConfig
    {
        public List<HeatConfig> HeatConfigs { get; set; } = [];
    }

    public class HeatConfig
    {
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; set; }
        public int GroupCount { get; set; }
        public int ParticipantsPerGroup { get; set; }
        public ScoringMode ScoringMode { get; set; }

        public ScoringConfig Scoring { get; set; } = new();
        public AssignmentConfig Assignment { get; set; } = new();
    }

    public class ScoringConfig
    {
        public IScoreAssignmentMethod Method { get; set; }
        public bool UsePenalties { get; set; }
    }

    public class AssignmentConfig
    {
        public IKartNummerAssignmentMethod KartMethod { get; set; }
        public IGroupAssignmentMethod GroupMethod { get; set; }
        public IParticipantShuffleMethod Shuffle { get; set; }
    }

    public enum ScoringMode
    {
        TimeBased,    
        PointsBased,  
        Hybrid        
    }
}
