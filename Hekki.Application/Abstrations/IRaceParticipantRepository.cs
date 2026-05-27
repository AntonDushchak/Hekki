using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRaceParticipantRepository
    {
        Task<IReadOnlyList<RaceParticipant>> GetAllAsync(CancellationToken ct = default);
        Task<RaceParticipant?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<RaceParticipant>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Get race participants with pilot information for service layer
        /// </summary>
        Task<IReadOnlyList<ParticipantWithPilotData>> GetByRaceIdWithPilotsAsync(int raceId, CancellationToken ct = default);

        Task<int> AddAsync(RaceParticipant participant, CancellationToken ct = default);
        Task UpdateAsync(RaceParticipant participant, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository DTO for participant with pilot data (used internally by service layer)
    /// </summary>
    public class ParticipantWithPilotData
    {
        public int ParticipantId { get; set; }
        public int RaceId { get; set; }
        public int PilotId { get; set; }
        public string? Team { get; set; }
        public bool IsActive { get; set; }

        // Pilot information
        public string PilotName { get; set; } = string.Empty;
        public string? PilotProfileUrl { get; set; }
        public string? PilotPhotoPath { get; set; }
    }
}
