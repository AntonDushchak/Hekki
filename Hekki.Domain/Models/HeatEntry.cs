using System.Xml;

namespace Hekki.Domain.Models
{
    public class HeatEntry
    {
        public int GroupNumber { get; set; }
        public int HeatId { get; set; }
        public int ParticipantId { get; set; }
        public int SeedOrder { get; set; }
        public int? GridPosition { get; set; }
        public int? KartNumber { get; set; }
    }
}
