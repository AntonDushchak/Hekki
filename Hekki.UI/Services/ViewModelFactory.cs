using Hekki.Application.Abstrations;
using Hekki.UI.ViewModels;
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
            return _serviceProvider.GetRequiredService<T>();
        }

        public RaceViewModel CreateRaceViewModel(int regulationId, int? raceId = null)
        {
            var raceService = _serviceProvider.GetRequiredService<IRaceService>();
            var pilotService = _serviceProvider.GetRequiredService<IPilotService>();
            return new RaceViewModel(regulationId, raceId, raceService, pilotService);
        }
    }

    public interface IViewModelFactory
    {
        T Create<T>() where T : class;
        RaceViewModel CreateRaceViewModel(int regulationId, int? raceId = null);
    }
}
