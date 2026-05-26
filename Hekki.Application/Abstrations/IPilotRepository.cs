using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IPilotRepository
    {
        Task<IReadOnlyList<Pilot>> GetAllAsync(CancellationToken ct = default);
        Task<Pilot?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(Pilot pilot, CancellationToken ct = default);
        Task UpdateAsync(Pilot pilot, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
