using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class HeatEntryMapper
    {
        public static HeatResultDto ToDto(HeatEntryEntity entity) => new()
        {
            ParticipantId = entity.ParticipantId,
            KartNumber = entity.KartNumber,
            GridPosition = entity.GridPosition
        };

        public static HeatEntryEntity ToEntity(HeatResultDto dto, int heatId, int groupNumber) => new()
        {
            HeatId = heatId,
            GroupNumber = groupNumber,
            ParticipantId = dto.ParticipantId,
            KartNumber = dto.KartNumber,
            GridPosition = dto.GridPosition,
            SeedOrder = 0
        };

        public static void UpdateEntity(HeatEntryEntity entity, HeatResultDto dto)
        {
            entity.KartNumber = dto.KartNumber;
            entity.GridPosition = dto.GridPosition;
        }
    }
}
