using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IHeatEntryRepository
    {
        Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAsync(int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAndGroupAsync(int heatId, int groupNumber, CancellationToken ct = default);
        Task AddAsync(int heatId, int groupNumber, HeatResultDto entry, CancellationToken ct = default);
        Task UpdateAsync(int heatId, int groupNumber, HeatResultDto entry, CancellationToken ct = default);
        Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default);
        Task<bool> ExistsAsync(int heatId, int participantId, CancellationToken ct = default);
    }
}
