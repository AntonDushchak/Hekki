using Hekki.Application.DTOs.Race;
using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.Abstrations
{
    public interface IRaceService
    {
        Task<RaceDataDto?> GetRaceDataAsync(int raceId, CancellationToken ct = default);
        Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default);
        Task<RaceParticipantDto> AddParticipantAsync(int raceId, int pilotId, CancellationToken ct = default);
        Task RemoveParticipantAsync(int participantId, CancellationToken ct = default);
        Task<RegulationEditDto?> GetRegulationEditAsync(int regulationId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GenerateHeatsAsync(int raceId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatGroupDto>> GenerateGroupsAsync(int raceId, int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GenerateHeatsWithGroupsAsync(int raceId, CancellationToken ct = default);
        Task<IReadOnlyList<GroupAssignmentResultDto>> AssignGroupsAndNumbersAsync(int raceId, int heatNumber, CancellationToken ct = default);
    }
}
