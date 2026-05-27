using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IHeatEntryRepository
    {
        Task<IReadOnlyList<HeatEntry>> GetByHeatIdAsync(int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatEntry>> GetByHeatIdsAsync(IEnumerable<int> heatIds, CancellationToken ct = default);
        Task AddAsync(HeatEntry entry, CancellationToken ct = default);
        Task UpdateAsync(HeatEntry entry, CancellationToken ct = default);
        Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default);
    }
}
