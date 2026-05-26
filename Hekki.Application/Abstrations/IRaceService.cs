using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRaceService
    {
        /// <summary>
        /// Get race by ID
        /// </summary>
        Task<Race?> GetRaceByIdAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Create new race with heats based on regulation
        /// </summary>
        Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default);

        /// <summary>
        /// Get all heats for a race
        /// </summary>
        Task<IReadOnlyList<Heat>> GetRaceHeatsAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Get regulation for a race
        /// </summary>
        Task<Regulation?> GetRaceRegulationAsync(int regulationId, CancellationToken ct = default);

        /// <summary>
        /// Get all participants for a race with pilot information
        /// </summary>
        Task<IReadOnlyList<RaceParticipantWithPilot>> GetRaceParticipantsAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Get heat entries for a specific heat
        /// </summary>
        Task<IReadOnlyList<HeatEntry>> GetHeatEntriesAsync(int heatId, CancellationToken ct = default);

        /// <summary>
        /// Get heat results for a specific heat
        /// </summary>
        Task<IReadOnlyList<HeatParticipantResult>> GetHeatResultsAsync(int heatId, CancellationToken ct = default);

        /// <summary>
        /// Add participant to race
        /// </summary>
        Task<int> AddParticipantAsync(int raceId, int pilotId, string team, CancellationToken ct = default);

        /// <summary>
        /// Remove participant from race
        /// </summary>
        Task RemoveParticipantAsync(int participantId, CancellationToken ct = default);

        /// <summary>
        /// Check if pilot is already participant in race
        /// </summary>
        Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default);
    }

    /// <summary>
    /// Aggregated model combining RaceParticipant with Pilot information
    /// </summary>
    public class RaceParticipantWithPilot
    {
        public RaceParticipant Participant { get; set; } = null!;

        // Pilot information
        public string PilotName { get; set; } = string.Empty;
        public string? PilotProfileUrl { get; set; }
        public string? PilotPhotoPath { get; set; }
    }
}
