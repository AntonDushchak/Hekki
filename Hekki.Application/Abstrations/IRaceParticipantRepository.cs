using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IRaceParticipantRepository
    {
        Task<IReadOnlyList<PilotDto>> GetAllAsync(CancellationToken ct = default);
        Task<PilotDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<PilotDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(int raceId, PilotDto participant, CancellationToken ct = default);
        Task UpdateAsync(PilotDto participant, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
