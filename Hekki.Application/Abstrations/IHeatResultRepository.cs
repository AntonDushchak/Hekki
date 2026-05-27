using Hekki.Domain;
using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IHeatResultRepository
    {
        Task<IReadOnlyList<HeatResult>> GetByHeatIdAsync(int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatResult>> GetByHeatIdsAsync(IEnumerable<int> heatIds, CancellationToken ct = default);
        Task<IReadOnlyList<HeatResultWithPilotInfo>> GetByHeatIdsWithPilotInfoAsync(IEnumerable<int> heatIds, CancellationToken ct = default);
        Task AddAsync(HeatResult result, CancellationToken ct = default);
        Task UpdateAsync(HeatResult result, CancellationToken ct = default);
        Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default);
    }

    /// <summary>
    /// DTO combining HeatResult with Pilot information from RaceParticipant
    /// </summary>
    public class HeatResultWithPilotInfo
    {
        public int HeatId { get; set; }
        public int ParticipantId { get; set; }
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
        public ResultStatus Status { get; set; }

        // Pilot information (from RaceParticipant -> Pilot join)
        public string PilotName { get; set; } = string.Empty;
    }
}
