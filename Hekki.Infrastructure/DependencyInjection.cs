using Hekki.Application.Abstrations;
using Hekki.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hekki.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<HekkiDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("HekkiDb")));

            services.AddAutoMapper(cfg => cfg.AddProfile<Mapping.MappingProfile>());

            services.AddTransient<IRegulationRepository, RegulationRepository>();
            services.AddTransient<IPilotRepository, PilotRepository>();
            services.AddTransient<IRaceParticipantRepository, RaceParticipantRepository>();
            services.AddTransient<IHeatRepository, HeatRepository>();
            services.AddTransient<IRaceRepository, RaceRepository>();

            return services;
        }
    }
}
