using Hekki.Domain.Models;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mappers
{
    public static class PilotMapper
    {
        public static Pilot ToDomain(this PilotEntity entity)
        {
            return new Pilot
            {
                Id = entity.Id,
                Name = entity.Name,
                ProfileUrl = entity.ProfileUrl,
                PhotoPath = entity.PhotoPath
            };
        }

        public static PilotEntity ToEntity(this Pilot domain)
        {
            return new PilotEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                ProfileUrl = domain.ProfileUrl,
                PhotoPath = domain.PhotoPath
            };
        }

        public static void UpdateEntity(this Pilot domain, PilotEntity entity)
        {
            entity.Name = domain.Name;
            entity.ProfileUrl = domain.ProfileUrl;
            entity.PhotoPath = domain.PhotoPath;
        }
    }
}
