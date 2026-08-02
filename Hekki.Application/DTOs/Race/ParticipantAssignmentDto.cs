namespace Hekki.Application.DTOs.Race
{
    public record ParticipantAssignmentDto
    {
        public int ParticipantId { get; init; }
        public int HeatId { get; init; }
        public int GroupId { get; init; }
        public int GridPosition { get; init; }
        public int KartNumber { get; init; }
    }
}
