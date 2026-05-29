using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Hekki.Application.Services;
using Hekki.Infrastructure;
using Hekki.Infrastructure.Repositories;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;
using Hekki.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Threading;

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
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.AddDebug();
                    });


                    services.AddDbContextFactory<HekkiDbContext>(options =>
                        options.UseNpgsql(context.Configuration.GetConnectionString("HekkiDb")));

                    // AutoMapper
                    services.AddAutoMapper(typeof(Hekki.Infrastructure.Mapping.MappingProfile));

                    services.AddTransient<IRegulationRepository, RegulationRepository>();


                    AddServicesMethods(services);

                    services.AddTransient<IRegulationService, RegulationService>();

                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddTransient<IPaginationService, PaginationService>();

                    services.AddTransient<IViewModelFactory, ViewModelFactory>();

                    services.AddTransient<IPilotRepository, PilotRepository>();
                    services.AddTransient<IRaceParticipantRepository, RaceParticipantRepository>();
                    services.AddTransient<IHeatRepository, HeatRepository>();
                    services.AddTransient<IRaceRepository, RaceRepository>();
                    services.AddTransient<IHeatResultRepository, HeatResultRepository>();
                    services.AddTransient<IHeatEntryRepository, HeatEntryRepository>();


                    services.AddTransient<IPilotService, PilotService>();
                    services.AddTransient<IRaceService, RaceService>();


                    services.AddSingleton<RegulationPickerViewModel>();
                    services.AddTransient<SelectionViewModel>();
                    services.AddTransient<CreateRegulationViewModel>();

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


            services.AddSingleton<IMethodCatalogService, MethodCatalogService>();

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            SetupGlobalExceptionHandling();
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

        private void SetupGlobalExceptionHandling()
        {
            WeakReferenceMessenger.Default.Register<AppErrorMessage>(this, (r, m) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var errorWin = new ErrorWindow(m.Message);
                    errorWin.Owner = Current.MainWindow;
                    errorWin.ShowDialog();
                });
            });

            this.DispatcherUnhandledException += (s, e) =>
            {
                UiServices?.GetService<ILogger<App>>()?.LogError(e.Exception, "UI exception");
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(e.Exception.Message));
                e.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                UiServices?.GetService<ILogger<App>>()?.LogCritical(ex, "Critical exception");
                if (!e.IsTerminating && ex != null)
                    WeakReferenceMessenger.Default.Send(new AppErrorMessage(ex.Message));
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                UiServices?.GetService<ILogger<App>>()?.LogError(e.Exception, "Task exception");
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(e.Exception.Message));
                e.SetObserved();
            };
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
