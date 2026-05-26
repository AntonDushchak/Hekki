using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRaceParticipantRepository
    {
        Task<IReadOnlyList<RaceParticipant>> GetAllAsync(CancellationToken ct = default);
        Task<RaceParticipant?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<RaceParticipant>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(RaceParticipant participant, CancellationToken ct = default);
        Task UpdateAsync(RaceParticipant participant, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default);
    }
}
