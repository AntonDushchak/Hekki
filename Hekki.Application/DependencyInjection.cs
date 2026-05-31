using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Hekki.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hekki.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IRegulationService, RegulationService>();
            services.AddTransient<IPilotService, PilotService>();
            services.AddTransient<IRaceService, RaceService>();

            services.AddSingleton<IParticipantShuffleMethod, NoShuffle>();
            services.AddSingleton<IParticipantShuffleMethod, RandomShuffle>();
            services.AddSingleton<IParticipantShuffleMethod, ScoreAscShuffle>();
            services.AddSingleton<IParticipantShuffleMethod, TimeDescShuffle>();
            services.AddSingleton<IParticipantShuffleCatalog, ParticipantShuffleCatalog>();

            services.AddSingleton<IGroupAssignmentMethod, RandomGroupAssignment>();
            services.AddSingleton<IGroupAssignmentMethod, CardGroupAssignment>();
            services.AddSingleton<IGroupAssignmentMethod, ListGroupAssignment>();
            services.AddSingleton<IGroupAssignmentMethod, ReplacementGroupAssignment>();
            services.AddSingleton<IGroupAssignmentCatalog, GroupAssigmentCatalog>();

            services.AddSingleton<IKartNummerAssignmentMethod, RandomKartAssignment>();
            services.AddSingleton<IKartNummerAssignmentMethod, RandomNoRepeatKartAssignment>();
            services.AddSingleton<IKartNummerAssignmentCatalog, KartNummerAssigmentCatalog>();

            services.AddSingleton<IScoreAssignmentMethod, DefaultScoreAssignment>();
            services.AddSingleton<IScoreAssignmentCatalog, ScoreAssignmentCatalog>();


            return services;
        }
    }
}
