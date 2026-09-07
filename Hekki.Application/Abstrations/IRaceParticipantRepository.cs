using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Abstrations
{
    public interface IRaceParticipantRepository
    {
        Task<RaceParticipantDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<RaceParticipantDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<Guid> AddAsync(RaceParticipantDto participant, int raceId, CancellationToken ct = default);
        Task UpdateAsync(RaceParticipantDto participant, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}
