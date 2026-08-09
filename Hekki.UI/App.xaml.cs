using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application;
using Hekki.Infrastructure;
using Hekki.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Threading;
using Hekki.Application.Messages.App;

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
            var main = _uiScope.ServiceProvider.GetRequiredService<MainWindow>();
            main.Show();
        }

        private void SetupGlobalExceptionHandling()
        {
            WeakReferenceMessenger.Default.Register<AppErrorMessage>(this, (r, m) =>
            {
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
