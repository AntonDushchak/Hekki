namespace Hekki.Application.DTOs.Race
{
    public record HeatGroupDto
    {
        public int HeatId { get; init; }
        public int GroupIndex { get; init; }
        public int GroupNumber { get; init; }
        public int GroupCapacity { get; init; }
        public IReadOnlyList<HeatEntryDto> Entries { get; init; } = [];
        public IReadOnlyList<HeatResultDto> Results { get; init; } = [];
    }
}
