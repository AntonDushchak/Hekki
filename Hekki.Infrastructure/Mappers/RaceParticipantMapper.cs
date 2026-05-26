using Hekki.Domain.Models;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class RaceParticipantMapper
    {
        public static RaceParticipant ToDomain(this RaceParticipantEntity entity)
        {
            return new RaceParticipant
            {
                Id = entity.Id,
                RaceId = entity.RaceId,
                PilotId = entity.PilotId,
                Team = entity.Team,
                IsActive = entity.IsActive
            };
        }

        public static RaceParticipantEntity ToEntity(this RaceParticipant domain)
        {
            return new RaceParticipantEntity
            {
                Id = domain.Id,
                RaceId = domain.RaceId,
                PilotId = domain.PilotId,
                Team = domain.Team,
                IsActive = domain.IsActive
            };
        }

        public static void UpdateEntity(this RaceParticipant domain, RaceParticipantEntity entity)
        {
            entity.Team = domain.Team;
            entity.IsActive = domain.IsActive;
        }
    }
}
