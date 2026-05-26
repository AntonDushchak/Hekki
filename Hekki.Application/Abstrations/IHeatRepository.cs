using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IHeatRepository
    {
        Task<IReadOnlyList<Heat>> GetAllAsync(CancellationToken ct = default);
        Task<Heat?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Heat>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(Heat heat, CancellationToken ct = default);
        Task UpdateAsync(Heat heat, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
