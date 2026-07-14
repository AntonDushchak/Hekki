using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Abstrations
{
    public interface IRaceParticipantRepository
    {
        Task<IReadOnlyList<RaceParticipantDto>> GetAllAsync(CancellationToken ct = default);
        Task<RaceParticipantDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<RaceParticipantDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(int raceId, RaceParticipantDto participant, CancellationToken ct = default);
        Task UpdateAsync(RaceParticipantDto participant, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
