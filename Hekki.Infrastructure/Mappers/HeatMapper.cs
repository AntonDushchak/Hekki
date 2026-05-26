using Hekki.Domain.Models;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class HeatMapper
    {
        // Heat mappings
        public static Heat ToHeatDomain(this HeatEntity entity)
        {
            return new Heat
            {
                Id = entity.Id,
                RaceId = entity.RaceId,
                Name = entity.Name,
                RoleLabel = entity.RoleLabel,
                RegulationId = entity.RegulationId,
                ConfigurationIndex = entity.ConfigurationIndex,
                //Status = entity.Status
            };
        }

        public static HeatEntity ToHeatEntity(this Heat domain)
        {
            return new HeatEntity
            {
                Id = domain.Id,
                RaceId = domain.RaceId,
                Name = domain.Name,
                RoleLabel = domain.RoleLabel,
                RegulationId = domain.RegulationId,
                ConfigurationIndex = domain.ConfigurationIndex,
                //Status = domain.Status
            };
        }

        public static void UpdateHeatEntity(this Heat domain, HeatEntity entity)
        {
            entity.Name = domain.Name;
            entity.RoleLabel = domain.RoleLabel;
            entity.RegulationId = domain.RegulationId;
            entity.ConfigurationIndex = domain.ConfigurationIndex;
            //entity.Status = domain.Status;
        }

        // HeatEntry mappings
        public static HeatEntry ToDomain(this HeatEntryEntity entity)
        {
            return new HeatEntry
            {
                HeatId = entity.HeatId,
                ParticipantId = entity.ParticipantId,
                SeedOrder = entity.SeedOrder,
                GridPosition = entity.GridPosition,
                KartNumber = entity.KartNumber
            };
        }

        public static HeatEntryEntity ToEntity(this HeatEntry domain)
        {
            return new HeatEntryEntity
            {
                HeatId = domain.HeatId,
                ParticipantId = domain.ParticipantId,
                SeedOrder = domain.SeedOrder,
                GridPosition = domain.GridPosition,
                KartNumber = domain.KartNumber
            };
        }

        // HeatParticipantResult mappings
        public static HeatParticipantResult ToResultDomain(this HeatResultEntity entity)
        {
            return new HeatParticipantResult
            {
                HeatId = entity.HeatId,
                ParticipantId = entity.ParticipantId,
                FinishPosition = entity.FinishPosition,
                TotalTimeMs = entity.TotalTimeMs,
                BestLapMs = entity.BestLapMs,
                Laps = entity.Laps,
                Status = entity.Status
            };
        }

        public static HeatResultEntity ToResultEntity(this HeatParticipantResult domain)
        {
            return new HeatResultEntity
            {
                HeatId = domain.HeatId,
                ParticipantId = domain.ParticipantId,
                FinishPosition = domain.FinishPosition,
                TotalTimeMs = domain.TotalTimeMs,
                BestLapMs = domain.BestLapMs,
                Laps = domain.Laps,
                Status = domain.Status
            };
        }
    }
}
