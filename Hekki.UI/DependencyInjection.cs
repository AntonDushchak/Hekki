using Hekki.Application.Abstractions;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;
using Hekki.UI.ViewModels.Race.Session;
using Hekki.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Hekki.UI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IPaginationService, PaginationService>();
            services.AddTransient<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IMethodCatalogService, MethodCatalogService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IEventPublisher, MessengerEventPublisher>();
            services.AddSingleton<AppSettings>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<RaceSessionHolder>();



            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>();

            return services;
        }
    }
}
