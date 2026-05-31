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
        /// Create new race with heats based on regulation
        /// </summary>
        Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default);

        /// <summary>
        /// Add participant to race
        /// </summary>
        Task<int> AddParticipantAsync(int raceId, int pilotId, CancellationToken ct = default);

        /// <summary>
        /// Remove participant from race
        /// </summary>
        Task RemoveParticipantAsync(int participantId, CancellationToken ct = default);

        Task<RegulationEditDto> GetRegulationEditAsync(int regulationId, CancellationToken ct = default);
    }
}
