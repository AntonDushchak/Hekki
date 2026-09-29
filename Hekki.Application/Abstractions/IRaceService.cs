using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.Abstractions
{
    public interface IRaceService
    {
        Task<RaceDataDto?> GetRaceDataAsync(int raceId, CancellationToken ct = default);
        Task<IReadOnlyList<RaceSummaryDto>> GetRacesSinceAsync(DateTime since, CancellationToken ct = default);
        Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default);
        Task UpdateRaceAsync(int raceId, string name, string location, DateTime date, CancellationToken ct = default);
        Task<RaceParticipantDto> AddParticipantAsync(int raceId, int pilotId, CancellationToken ct = default);
        Task RemoveParticipantAsync(int raceId, Guid participantId, CancellationToken ct = default);
        Task<RaceParticipantDto> UpdateParticipantPilotAsync(int raceId, Guid participantId, PilotDto pilot, CancellationToken ct = default);
        Task ReorderParticipantsAsync(int raceId, IReadOnlyList<Guid> orderedIds, CancellationToken ct = default);
        Task<HeatResultDto> SetHeatResultValueAsync(int raceId, int heatId, Guid participantId, HeatResultField field, long? value, CancellationToken ct = default);
        Task<RegulationEditDto?> GetRegulationEditAsync(int regulationId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GenerateHeatsAsync(int raceId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatGroupDto>> GenerateGroupsAsync(int raceId, int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GenerateHeatsWithGroupsAsync(int raceId, CancellationToken ct = default);
        Task<IReadOnlyList<GroupAssignmentResultDto>> AssignGroupsAndNumbersAsync(int raceId, int heatNumber, CancellationToken ct = default);
        Task ClearHeatAssignmentAsync(int raceId, int heatId, CancellationToken ct = default);
        Task<HeatConfig> GetHeatConfigAsync(int raceId, int heatId, CancellationToken ct = default);
        Task UpdateHeatConfigAsync(int raceId, int heatId, HeatConfig config, CancellationToken ct = default);
    }
}
