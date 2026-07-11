namespace Hekki.Infrastructure.Entities
{
    public class HeatGroupEntity
    {
        public int Id { get; set; }
        public int HeatId { get; set; }
        public int GroupIndex { get; set; }
        public int GroupNumber { get; set; }
        public int GroupCapacity { get; set; }

        public HeatEntity Heat { get; set; } = null!;
        public List<HeatEntryEntity> Entries { get; set; } = [];
        public List<HeatResultEntity> Results { get; set; } = [];
    }
}
