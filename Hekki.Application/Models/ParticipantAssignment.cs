namespace Hekki.Application.Models
{
    public sealed class ParticipantAssignment
    {
        public int ParticipantId { get; init; }
        public int HeatId { get; set; }
        public int GroupId { get; set; }
        public int GridPosition { get; init; }
        public int KartNumber { get; set; }
    }
}
