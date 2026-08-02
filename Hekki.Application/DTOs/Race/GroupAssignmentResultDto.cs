namespace Hekki.Application.DTOs.Race
{
    public record GroupAssignmentResultDto
    {
        public required HeatGroupDto Group { get; init; }
        public required IReadOnlyList<HeatEntryDto> UpdatedEntries { get; init; }
    }
}
