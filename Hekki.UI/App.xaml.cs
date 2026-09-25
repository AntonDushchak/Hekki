using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application;
using Hekki.Infrastructure;
using Hekki.UI.Services;
using Hekki.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Threading;
using Hekki.UI.Messages;

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

                    services.AddInfrastructure(context.Configuration);
                    services.AddApplication();
                    services.AddPresentation();
                })
                .Build();
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
            await UiServices.GetRequiredService<IAppSettingsService>().LoadAsync();
            var main = _uiScope.ServiceProvider.GetRequiredService<MainWindow>();
            main.Show();
        }

        private void SetupGlobalExceptionHandling()
        {
            WeakReferenceMessenger.Default.Register<AppErrorMessage>(this, (r, m) =>
            {
                if (m.Exception != null)
                    Logger?.LogError(m.Exception, "Error in {Source}", m.Source ?? "unknown");

                if (Dispatcher.CheckAccess())
                {
                    ShowMessageWindow(m.Message, MessageType.Error);
                }
                else
                {
                    Dispatcher.Invoke(() => ShowMessageWindow(m.Message, MessageType.Error));
                }
            });

            WeakReferenceMessenger.Default.Register<AppSuccessMessage>(this, (r, m) =>
            {
                if (Dispatcher.CheckAccess())
                {
                    ShowMessageWindow(m.Message, MessageType.Success);
                }
                else
                {
                    Dispatcher.Invoke(() => ShowMessageWindow(m.Message, MessageType.Success));
                }
            });

            WeakReferenceMessenger.Default.Register<AppInfoMessage>(this, (r, m) =>
            {
                if (Dispatcher.CheckAccess())
                {
                    ShowMessageWindow(m.Message, MessageType.Info);
                }
                else
                {
                    Dispatcher.Invoke(() => ShowMessageWindow(m.Message, MessageType.Info));
                }
            });

            WeakReferenceMessenger.Default.Register<AppWarningMessage>(this, (r, m) =>
            {
                if (Dispatcher.CheckAccess())
                {
                    ShowMessageWindow(m.Message, MessageType.Warning);
                }
                else
                {
                    Dispatcher.Invoke(() => ShowMessageWindow(m.Message, MessageType.Warning));
                }
            });

            this.DispatcherUnhandledException += (s, e) =>
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(Localizer.ForException(e.Exception), e.Exception, "Dispatcher"));
                e.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (e.IsTerminating || ex == null)
                {
                    Logger?.LogCritical(ex, "Critical exception");
                    return;
                }
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(Localizer.ForException(ex), ex, "AppDomain"));
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(Localizer.ForException(e.Exception), e.Exception, "TaskScheduler"));
                e.SetObserved();
            };
        }

        private static ILogger? Logger => Host?.Services.GetService<ILogger<App>>();

        private void ShowMessageWindow(string message, MessageType type)
        {
            var messageWindow = new ErrorWindow(message, type);

            if (Current?.MainWindow?.IsLoaded == true)
            {
                messageWindow.Owner = Current.MainWindow;
            }

            messageWindow.ShowDialog();
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
