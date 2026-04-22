using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Hekki.Infrastructure;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;
using Hekki.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Hekki.UI
{
    public partial class App : System.Windows.Application
    {
        public static IHost Host { get; private set; } = null!;
        private static IServiceScope _uiScope = null!;
        public static IServiceProvider UiServices => _uiScope.ServiceProvider;
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

                    services.AddTransient<IRegulationRepository, RegulationRepository>();


                    AddServicesMethods(services);

                    services.AddTransient<IRegulationService, RegulationService>();

                    services.AddSingleton<NavigationService>();
                    services.AddTransient<IPaginationService, PaginationService>();

                    services.AddTransient<IViewModelFactory, ViewModelFactory>();

                    //services.AddTransient<SelectRaceView>();
                    //services.AddTransient<CreateRaceView>();
                    //services.AddTransient<RaceView>();
                    services.AddTransient<SelectionViewModel>();
                    services.AddTransient<CreateRaceViewModel>();
                    services.AddTransient<RaceViewModel>();
                    //services.AddTransient<SelectionTopPanelViewModel>();

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
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
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HekkiDbContext>>();
                using var db = factory.CreateDbContext();
                await db.Database.MigrateAsync();
            }
            _uiScope = Host.Services.CreateScope();
            base.OnStartup(e);
            var main = _uiScope.ServiceProvider.GetRequiredService<MainWindow>();
            main.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _uiScope.Dispose();
            await Host.StopAsync();
            Host.Dispose();
            base.OnExit(e);
        }
    }

}
