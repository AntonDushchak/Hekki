namespace Hekki.Application.Regulations
{
    public class RegulationConfig
    {
        public ScoringConfig Scoring { get; set; } = new();
        public AssignmentConfig Assignment { get; set; } = new();
        public HeatStructureConfig HeatStructure { get; set; } = new();
    }

    public class ScoringConfig
    {
        public ScoreMethod Method { get; set; }
        public bool UsePenalties { get; set; }
    }

    public class AssignmentConfig
    {
        public KartAssignmentMethod KartMethod { get; set; }
        public GroupAssignmentMethod GroupMethod { get; set; }
        public ShuffleMethod Shuffle { get; set; }
    }

    public class HeatStructureConfig
    {
        public int HeatCount { get; set; }
        public int GroupCount { get; set; }
        public int ParticipantsPerGroup { get; set; }
    }
}
