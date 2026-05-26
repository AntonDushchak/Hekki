using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRaceRepository
    {
        Task<IReadOnlyList<Race>> GetAllAsync(CancellationToken ct = default);
        Task<Race?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(Race race, CancellationToken ct = default);
        Task UpdateAsync(Race race, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
