using Hekki.Domain.Models;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class RaceMapper
    {
        public static Race ToDomain(this RaceEntity entity)
        {
            return new Race
            {
                Id = entity.Id,
                Date = entity.Date,
                Location = entity.Location,
                Name = entity.Name,
                DefaultRegulationId = entity.RegulationId
            };
        }

        public static RaceEntity ToEntity(this Race domain)
        {
            return new RaceEntity
            {
                Id = domain.Id,
                Date = domain.Date,
                Location = domain.Location,
                Name = domain.Name,
                RegulationId = domain.DefaultRegulationId
            };
        }

        public static void UpdateEntity(this Race domain, RaceEntity entity)
        {
            entity.Date = domain.Date;
            entity.Location = domain.Location;
            entity.Name = domain.Name;
            entity.RegulationId = domain.DefaultRegulationId;
        }
    }
}
