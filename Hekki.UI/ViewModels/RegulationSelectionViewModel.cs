using Hekki.Application.Abstrations;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public class RegulationSelectionViewModel
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NavigationService _navigationService;
        public ObservableCollection<RegulationChoiceItem> Items { get; } = [];

        private RegulationChoiceItem? _selectedItem;
        private bool _initialized = false;

        public RegulationChoiceItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (value is null) return;

                if (value.Kind == RegulationChoiceKind.Create)
                    _navigationService.Navigate(new Uri("/Pages/RegulationCreation.xaml", UriKind.Relative));
                else
                    _navigationService.NavigateToRace(value.RegulationId!.Value);
            }
        }

        public RegulationSelectionViewModel(IServiceScopeFactory scopeFactory, NavigationService navigationService)
        {
            _scopeFactory = scopeFactory;
            _navigationService = navigationService;
        }

        public void NavigateSelected()
        {
            var value = SelectedItem;
            if (value is null) return;

            if (value.Kind == RegulationChoiceKind.Create)
                _navigationService.Navigate(new Uri("/Pages/RegulationCreation.xaml", UriKind.Relative));
            else
                _navigationService.NavigateToRace(value.RegulationId!.Value);
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;
            _initialized = true;

            Items.Clear();
            Items.Add(new("Create regulation…", RegulationChoiceKind.Create, null));

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRegulationRepository>();
            var regulations = await repo.GetAllAsync();
            foreach (var r in regulations)
                Items.Add(new(r.Name, RegulationChoiceKind.Existing, r.Id));
        }
    }
}
