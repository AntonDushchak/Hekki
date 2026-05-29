using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IHeatResultRepository
    {
        Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAsync(int heatId, CancellationToken ct = default);
        Task<HeatResultDto?> GetByHeatAndParticipantAsync(int heatId, int participantId, CancellationToken ct = default);
        Task AddAsync(int heatId, HeatResultDto result, CancellationToken ct = default);
        Task UpdateAsync(int heatId, HeatResultDto result, CancellationToken ct = default);
        Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default);
        Task<bool> ExistsAsync(int heatId, int participantId, CancellationToken ct = default);
    }
}
