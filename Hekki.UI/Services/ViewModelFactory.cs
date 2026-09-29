using Hekki.UI.ViewModels;
using Hekki.UI.ViewModels.Race.Session;
using Hekki.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Hekki.UI.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : class
        {
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        public RaceViewModel CreateRaceViewModel(int regulationId, int? raceId = null)
        {
            return new RaceViewModel(
                regulationId,
                raceId,
                _serviceProvider.GetRequiredService<IRaceService>(),
                _serviceProvider.GetRequiredService<IPilotService>(),
                _serviceProvider.GetRequiredService<IDialogService>(),
                _serviceProvider.GetRequiredService<AppSettings>(),
                _serviceProvider.GetRequiredService<IMethodCatalogService>(),
                _serviceProvider.GetRequiredService<RaceSessionHolder>());
        }
    }

    public interface IViewModelFactory
    {
        T Create<T>() where T : class;
        RaceViewModel CreateRaceViewModel(int regulationId, int? raceId = null);
    }
}
