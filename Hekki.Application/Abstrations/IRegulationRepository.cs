using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRegulationRepository
    {
        Task<IReadOnlyList<Regulation>> GetAllAsync(CancellationToken ct = default);
        Task<Regulation?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(Regulation regulation, CancellationToken ct = default);
        Task UpdateAsync(Regulation regulation, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
