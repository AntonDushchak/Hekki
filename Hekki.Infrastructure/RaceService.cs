using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Domain.Models;

namespace Hekki.Infrastructure
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IHeatRepository _heatRepository;
        private readonly IRegulationRepository _regulationRepository;
        private readonly IRaceParticipantRepository _raceParticipantRepository;
        private readonly IHeatResultRepository _heatResultRepository;
        private readonly IHeatEntryRepository _heatEntryRepository;

        public RaceService(
            IRaceRepository raceRepository,
            IHeatRepository heatRepository,
            IRegulationRepository regulationRepository,
            IRaceParticipantRepository raceParticipantRepository,
            IHeatResultRepository heatResultRepository,
            IHeatEntryRepository heatEntryRepository)
        {
            _raceRepository = raceRepository;
            _heatRepository = heatRepository;
            _regulationRepository = regulationRepository;
            _raceParticipantRepository = raceParticipantRepository;
            _heatResultRepository = heatResultRepository;
            _heatEntryRepository = heatEntryRepository;
        }

        public async Task<RaceDataDto> GetRaceDataAsync(int raceId, CancellationToken ct = default)
        {
            var race = await _raceRepository.GetByIdAsync(raceId, ct);
            if (race == null)
                throw new InvalidOperationException($"Race with ID {raceId} not found");

            var participants = await _raceParticipantRepository.GetByRaceIdWithPilotsAsync(raceId, ct);
            var activeParticipants = participants.Where(p => p.IsActive).OrderBy(p => p.PilotName).ToList();

            var heats = await _heatRepository.GetByRaceIdAsync(raceId, ct);
            var heatIds = heats.Select(h => h.Id).ToList();

            var heatResults = await _heatResultRepository.GetByHeatIdsWithPilotInfoAsync(heatIds, ct);

            var heatEntries = await _heatEntryRepository.GetByHeatIdsAsync(heatIds, ct);

            var pilotDtos = activeParticipants.Select(p => new PilotDto
            {
                PilotId = p.PilotId,
                ParticipantId = p.ParticipantId,
                Name = p.PilotName,
                PhotoPath = p.PilotPhotoPath,
                Team = p.Team,
                KartNumbers = heatEntries
                    .Where(he => he.ParticipantId == p.ParticipantId && he.KartNumber.HasValue)
                    .Select(he => he.KartNumber!.Value.ToString())
                    .Distinct()
                    .ToList(),
                Statistics = new Dictionary<string, string>() // TODO: Calculate statistics
            }).ToList();

            var heatDtos = heats.Select(h =>
            {
                var results = heatResults.Where(hr => hr.HeatId == h.Id).ToList();

                return new HeatDto
                {
                    HeatId = h.Id,
                    Name = h.Name,
                    GroupNumber = h.ConfigurationIndex + 1,
                    HeatNumber = h.ConfigurationIndex + 1,
                    DynamicColumns = ["Time"], // TODO: Get from configuration
                    Results = results
                        .OrderBy(r => r.FinishPosition ?? int.MaxValue)
                        .Select((r, index) => new HeatResultDto
                        {
                            Position = r.FinishPosition ?? index + 1,
                            KartNumber = heatEntries
                                .FirstOrDefault(e => e.HeatId == h.Id && e.ParticipantId == r.ParticipantId)
                                ?.KartNumber?.ToString() ?? "-",
                            PilotName = r.PilotName,
                            DynamicData = new Dictionary<string, string>
                            {
                                ["Time"] = FormatTime(r.TotalTimeMs)
                            }
                        }).ToList()
                };
            }).ToList();

            return new RaceDataDto
            {
                RaceId = raceId,
                RaceName = race.Name,
                Participants = pilotDtos,
                Heats = heatDtos
            };
        }

        public async Task<IReadOnlyList<PilotDto>> SearchPilotsAsync(int raceId, string searchText, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return [];

            var participants = await _raceParticipantRepository.GetByRaceIdWithPilotsAsync(raceId, ct);

            var filteredParticipants = participants
                .Where(p => p.IsActive && p.PilotName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.PilotName)
                .Take(10)
                .ToList();

            return filteredParticipants.Select(p => new PilotDto
            {
                PilotId = p.PilotId,
                ParticipantId = p.ParticipantId,
                Name = p.PilotName,
                PhotoPath = p.PilotPhotoPath,
                Team = p.Team
            }).ToList();
        }

        public async Task<int> AddPilotToRaceAsync(int raceId, int pilotId, string? team = null, CancellationToken ct = default)
        {
            if (await IsParticipantInRaceAsync(raceId, pilotId, ct))
                throw new InvalidOperationException($"Pilot {pilotId} is already in race {raceId}");

            return await AddParticipantAsync(raceId, pilotId, team ?? string.Empty, ct);
        }

        public async Task RemoveParticipantFromRaceAsync(int raceId, int participantId, CancellationToken ct = default)
        {
            await RemoveParticipantAsync(participantId, ct);
        }

        private static string FormatTime(long? timeMs)
        {
            if (!timeMs.HasValue || timeMs.Value == 0)
                return "-";

            var ts = TimeSpan.FromMilliseconds(timeMs.Value);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}.{ts.Milliseconds:D3}";
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

        public async Task<IReadOnlyList<PilotDto>> GetRaceParticipantsAsync(int raceId, CancellationToken ct = default)
        {
            var participants = await _raceParticipantRepository.GetByRaceIdWithPilotsAsync(raceId, ct);

            return participants.Select(p => new PilotDto
            {
                PilotId = p.PilotId,
                ParticipantId = p.ParticipantId,
                Name = p.PilotName,
                PhotoPath = p.PilotPhotoPath,
                ProfileUrl = p.PilotProfileUrl,
                Team = p.Team
            }).ToList();
        }

        public async Task<IReadOnlyList<HeatEntry>> GetHeatEntriesAsync(int heatId, CancellationToken ct = default)
        {
            return await _heatEntryRepository.GetByHeatIdAsync(heatId, ct);
        }

        public async Task<IReadOnlyList<HeatParticipantResult>> GetHeatResultsAsync(int heatId, CancellationToken ct = default)
        {
            var results = await _heatResultRepository.GetByHeatIdAsync(heatId, ct);

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
