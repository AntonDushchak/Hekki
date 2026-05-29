using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class RaceParticipantMapper
    {
        public static PilotDto ToDto(RaceParticipantEntity entity) => new()
        {
            ParticipantId = entity.Id,
            PilotId = entity.PilotId,
            Name = entity.Pilot.Name,
            PhotoPath = entity.Pilot.PhotoPath,
            Team = entity.Team
        };

        public static RaceParticipantEntity ToEntity(PilotDto dto, int raceId) => new()
        {
            Id = dto.ParticipantId,
            RaceId = raceId,
            PilotId = dto.PilotId,
            Team = dto.Team,
            IsActive = true
        };

        public static void UpdateEntity(RaceParticipantEntity entity, PilotDto dto)
        {
            entity.Team = dto.Team;
        }
    }
}
