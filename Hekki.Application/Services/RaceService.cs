using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IRaceParticipantRepository _participantRepository;
        private readonly IPilotRepository _pilotRepository;
        private readonly IRegulationRepository _regulationRepository;

        public RaceService(
            IRaceRepository raceRepository, 
            IRaceParticipantRepository participantRepository, 
            IPilotRepository pilotRepository,
            IRegulationRepository regulationRepository)
        {
            _raceRepository = raceRepository;
            _participantRepository = participantRepository;
            _pilotRepository = pilotRepository;
            _regulationRepository = regulationRepository;
        }

        public async Task<RaceDataDto?> GetRaceDataAsync(int raceId, CancellationToken ct = default)
        {
            return await _raceRepository.GetByIdAsync(raceId, ct);
        }

        public async Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default)
        {
            var utcDate = date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);
            var race = new RaceDataDto { RaceName = name, Location = location, Date = utcDate, RegulationId = regulationId };
            return await _raceRepository.AddAsync(race, ct);
        }

        public async Task<RaceParticipantDto> AddParticipantAsync(int raceId, int pilotId, CancellationToken ct = default)
        {
            var pilot = await _pilotRepository.GetByIdAsync(pilotId, ct);
            var participant = new RaceParticipantDto { PilotId = pilot.Id, Name = pilot.Name, Team = pilot.Team, IsActive = true };
            await _participantRepository.AddAsync(raceId, participant, ct);
            return participant;
        }

        public async Task RemoveParticipantAsync(int participantId, CancellationToken ct = default)
        {
            await _participantRepository.DeleteAsync(participantId, ct);
        }

        public async Task<RegulationEditDto?> GetRegulationEditAsync(int regulationId, CancellationToken ct = default)
        {
            return await _regulationRepository.GetForEditAsync(regulationId, ct);
        }
    }
}
