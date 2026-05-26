using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class RaceService : IRaceService
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IRaceRepository _raceRepository;
        private readonly IHeatRepository _heatRepository;
        private readonly IRegulationRepository _regulationRepository;
        private readonly IRaceParticipantRepository _raceParticipantRepository;

        public RaceService(
            IDbContextFactory<HekkiDbContext> dbFactory,
            IRaceRepository raceRepository,
            IHeatRepository heatRepository,
            IRegulationRepository regulationRepository,
            IRaceParticipantRepository raceParticipantRepository)
        {
            _dbFactory = dbFactory;
            _raceRepository = raceRepository;
            _heatRepository = heatRepository;
            _regulationRepository = regulationRepository;
            _raceParticipantRepository = raceParticipantRepository;
        }

        public async Task<Race?> GetRaceByIdAsync(int raceId, CancellationToken ct = default)
        {
            return await _raceRepository.GetByIdAsync(raceId, ct);
        }

        public async Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default)
        {
            var regulation = await _regulationRepository.GetByIdAsync(regulationId, ct);
            if (regulation == null)
                throw new InvalidOperationException($"Regulation with ID {regulationId} not found");

            var race = new Race
            {
                Name = name,
                Location = location,
                Date = date,
                DefaultRegulationId = regulationId
            };

            var raceId = await _raceRepository.AddAsync(race, ct);

            foreach (var config in regulation.Configurations)
            {
                var heat = new Heat
                {
                    RaceId = raceId,
                    RegulationId = regulationId,
                    Name = config.Name,
                    ConfigurationIndex = regulation.Configurations.IndexOf(config),
                    //Status = HeatStatus.NotStarted
                };

                await _heatRepository.AddAsync(heat, ct);
            }

            return raceId;
        }

        public async Task<IReadOnlyList<Heat>> GetRaceHeatsAsync(int raceId, CancellationToken ct = default)
        {
            return await _heatRepository.GetByRaceIdAsync(raceId, ct);
        }

        public async Task<Regulation?> GetRaceRegulationAsync(int regulationId, CancellationToken ct = default)
        {
            return await _regulationRepository.GetByIdAsync(regulationId, ct);
        }

        public async Task<IReadOnlyList<RaceParticipantWithPilot>> GetRaceParticipantsAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participants = await db.RaceParticipants
                .AsNoTracking()
                .Include(rp => rp.Pilot)
                .Where(rp => rp.RaceId == raceId)
                .OrderBy(rp => rp.Pilot.Name)
                .ToListAsync(ct);

            return participants.Select(rp => new RaceParticipantWithPilot
            {
                Participant = new RaceParticipant
                {
                    Id = rp.Id,
                    RaceId = rp.RaceId,
                    PilotId = rp.PilotId,
                    Team = rp.Team,
                    IsActive = rp.IsActive
                },
                PilotName = rp.Pilot.Name,
                PilotProfileUrl = rp.Pilot.ProfileUrl,
                PilotPhotoPath = rp.Pilot.PhotoPath
            }).ToList();
        }

        public async Task<IReadOnlyList<HeatEntry>> GetHeatEntriesAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .AsNoTracking()
                .Where(he => he.HeatId == heatId)
                .OrderBy(he => he.SeedOrder)
                .ToListAsync(ct);

            return entries.Select(e => new HeatEntry
            {
                HeatId = e.HeatId,
                ParticipantId = e.ParticipantId,
                SeedOrder = e.SeedOrder,
                GridPosition = e.GridPosition,
                KartNumber = e.KartNumber
            }).ToList();
        }

        public async Task<IReadOnlyList<HeatParticipantResult>> GetHeatResultsAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var results = await db.HeatResults
                .AsNoTracking()
                .Where(hr => hr.HeatId == heatId)
                .OrderBy(hr => hr.FinishPosition)
                .ToListAsync(ct);

            return results.Select(r => new HeatParticipantResult
            {
                HeatId = r.HeatId,
                ParticipantId = r.ParticipantId,
                FinishPosition = r.FinishPosition,
                TotalTimeMs = r.TotalTimeMs,
                BestLapMs = r.BestLapMs,
                Laps = r.Laps,
                Status = r.Status
            }).ToList();
        }

        public async Task<int> AddParticipantAsync(int raceId, int pilotId, string team, CancellationToken ct = default)
        {
            var participant = new RaceParticipant
            {
                RaceId = raceId,
                PilotId = pilotId,
                Team = team,
                IsActive = true
            };

            return await _raceParticipantRepository.AddAsync(participant, ct);
        }

        public async Task RemoveParticipantAsync(int participantId, CancellationToken ct = default)
        {
            await _raceParticipantRepository.DeleteAsync(participantId, ct);
        }

        public async Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default)
        {
            return await _raceParticipantRepository.IsParticipantInRaceAsync(raceId, pilotId, ct);
        }
    }
}
