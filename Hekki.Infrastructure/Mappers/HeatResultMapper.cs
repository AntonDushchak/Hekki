using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class HeatResultMapper
    {
        public static HeatResultDto ToDto(HeatResultEntity entity) => new()
        {
            ParticipantId = entity.ParticipantId,
            FinishPosition = entity.FinishPosition,
            TotalTimeMs = entity.TotalTimeMs,
            BestLapMs = entity.BestLapMs,
            Laps = entity.Laps
        };

        public static HeatResultEntity ToEntity(HeatResultDto dto, int heatId) => new()
        {
            HeatId = heatId,
            ParticipantId = dto.ParticipantId,
            FinishPosition = dto.FinishPosition,
            TotalTimeMs = dto.TotalTimeMs,
            BestLapMs = dto.BestLapMs,
            Laps = dto.Laps
        };

        public static void UpdateEntity(HeatResultEntity entity, HeatResultDto dto)
        {
            entity.FinishPosition = dto.FinishPosition;
            entity.TotalTimeMs = dto.TotalTimeMs;
            entity.BestLapMs = dto.BestLapMs;
            entity.Laps = dto.Laps;
        }
    }
}
