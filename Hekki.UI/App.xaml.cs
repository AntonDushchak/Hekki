using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Hekki.Infrastructure;
using Hekki.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost Host { get; private set; } = null!;

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContextFactory<HekkiDbContext>(options =>
                        options.UseNpgsql(context.Configuration.GetConnectionString("HekkiDb")));

                    services.AddScoped<IRegulationRepository, RegulationRepository>();
                    services.AddSingleton<RegulationSelectionViewModel>();
                    services.AddTransient<RegulationCreationViewModel>();
                    services.AddSingleton<NavigationService>();

                    
                    AddServicesMethods(services);

                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        private void AddServicesMethods(IServiceCollection services)
        {
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
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await Host.StartAsync();

            using (var scope = Host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HekkiDbContext>();
                await db.Database.MigrateAsync();
            }

            base.OnStartup(e);
            var main = Host.Services.GetRequiredService<MainWindow>();
            main.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Host.StopAsync();
            Host.Dispose();
            base.OnExit(e);
        }
    }

}
