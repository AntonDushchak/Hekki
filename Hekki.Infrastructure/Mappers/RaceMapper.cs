using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class RaceMapper
    {
        public static RaceDataDto ToDto(RaceEntity entity) => new()
        {
            RaceId = entity.Id,
            RaceName = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            RegulationId = entity.RegulationId,
            Participants = entity.Participants.Select(ToDto).ToList(),
            Heats = entity.Heats.Select(ToDto).ToList()
        };

        public static RaceEntity ToEntity(RaceDataDto dto) => new()
        {
            Id = dto.RaceId,
            Name = dto.RaceName,
            Date = dto.Date,
            Location = dto.Location,
            RegulationId = dto.RegulationId
        };

        public static void UpdateEntity(RaceEntity entity, RaceDataDto dto)
        {
            entity.Name = dto.RaceName;
            entity.Date = dto.Date;
            entity.Location = dto.Location;
        }

        public static PilotDto ToDto(RaceParticipantEntity p) => new()
        {
            PilotId = p.Pilot.Id,
            ParticipantId = p.Id,
            Name = p.Pilot.Name,
            PhotoPath = p.Pilot.PhotoPath,
            Team = p.Team
        };

        public static HeatDto ToDto(HeatEntity h) => new()
        {
            HeatId = h.Id,
            Name = h.Name,
            Groups = h.HeatEntries
                .GroupBy(e => e.GroupNumber)
                .Select(g => ToDto(g, h.HeatParticipantResults))
                .ToList()
        };

        public static HeatGroup ToDto(IGrouping<int, HeatEntryEntity> g, ICollection<HeatResultEntity> results) => new()
        {
            GroupNumber = g.Key,
            Results = g.Select(e => ToDto(e, results)).ToList()
        };

        public static HeatResultDto ToDto(HeatEntryEntity e, ICollection<HeatResultEntity> results)
        {
            var result = results.FirstOrDefault(r => r.ParticipantId == e.ParticipantId);
            return new()
            {
                ParticipantId = e.ParticipantId,
                KartNumber = e.KartNumber,
                GridPosition = e.GridPosition,
                FinishPosition = result?.FinishPosition,
                TotalTimeMs = result?.TotalTimeMs
            };
        }
    }
}
