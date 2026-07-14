namespace Hekki.Application.DTOs.Race
{
    public record HeatResultDto
    {
        public int ParticipantId { get; init; }
        public int? FinishPosition { get; init; }
        public long? TotalTimeMs { get; init; }
        public long? BestLapMs { get; init; }
        public int? Laps { get; init; }
        public int? Score { get; init; }
        public int? Penalty { get; init; }
        public int TotalScore => (Score ?? 0) - (Penalty ?? 0);
    }
}
