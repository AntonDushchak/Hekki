using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IRaceService
    {
        /// <summary>
        /// Get complete race data with participants and heats for UI
        /// </summary>
        Task<RaceDataDto> GetRaceDataAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Search pilots by name or kart number within race
        /// </summary>
        Task<IReadOnlyList<PilotDto>> SearchPilotsAsync(int raceId, string searchText, CancellationToken ct = default);

        /// <summary>
        /// Add pilot to race as participant
        /// </summary>
        Task<int> AddPilotToRaceAsync(int raceId, int pilotId, string? team = null, CancellationToken ct = default);

        /// <summary>
        /// Remove participant from race
        /// </summary>
        Task RemoveParticipantFromRaceAsync(int raceId, int participantId, CancellationToken ct = default);

        /// <summary>
        /// Create new race with heats based on regulation
        /// </summary>
        Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default);

        /// <summary>
        /// Get all heats for a race
        /// </summary>
        Task<IReadOnlyList<HeatDto>> GetRaceHeatsAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Get regulation for a race
        /// </summary>
        Task<RegulationEditDto?> GetRaceRegulationAsync(int regulationId, CancellationToken ct = default);

        /// <summary>
        /// Get all participants for a race with pilot information
        /// </summary>
        Task<IReadOnlyList<PilotDto>> GetRaceParticipantsAsync(int raceId, CancellationToken ct = default);

        /// <summary>
        /// Get heat entries for a specific heat
        /// </summary>
        Task<IReadOnlyList<HeatGroupDto>> GetHeatGroupsAsync(int heatId, CancellationToken ct = default);

        /// <summary>
        /// Get heat results for a specific heat
        /// </summary>
        Task<IReadOnlyList<HeatResultDto>> GetHeatResultsAsync(int heatId, CancellationToken ct = default);

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
}
