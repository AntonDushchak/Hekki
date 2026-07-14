namespace Hekki.Application.DTOs.Regulation
{
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
}
