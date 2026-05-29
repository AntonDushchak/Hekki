using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class HeatMapper
    {
        public static HeatDto ToDto(HeatEntity entity) => new()
        {
            HeatId = entity.Id,
            Name = entity.Name,
            Groups = entity.HeatEntries
                .GroupBy(e => e.GroupNumber)
                .Select(g => new HeatGroup
                {
                    GroupNumber = g.Key,
                    Results = g.Select(e => ToResultDto(e, entity.HeatParticipantResults)).ToList()
                })
                .ToList()
        };

        public static HeatEntity ToEntity(HeatDto dto) => new()
        {
            Id = dto.HeatId,
            Name = dto.Name
        };

        public static void UpdateEntity(HeatEntity entity, HeatDto dto)
        {
            entity.Name = dto.Name;
        }

        private static HeatResultDto ToResultDto(HeatEntryEntity entry, ICollection<HeatResultEntity> results)
        {
            var result = results.FirstOrDefault(r => r.ParticipantId == entry.ParticipantId);
            return new HeatResultDto
            {
                ParticipantId = entry.ParticipantId,
                KartNumber = entry.KartNumber,
                GridPosition = entry.GridPosition,
                FinishPosition = result?.FinishPosition,
                TotalTimeMs = result?.TotalTimeMs,
                BestLapMs = result?.BestLapMs,
                Laps = result?.Laps
            };
        }
    }
}
