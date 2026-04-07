using Hekki.Application.Abstrations;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public class RegulationSelectionViewModel
    {
        private readonly IRegulationRepository _regulationRepository;
        private readonly NavigationService _navigationService;
        public ObservableCollection<RegulationChoiceItem> Items { get; } = [];

        private RegulationChoiceItem? _selectedItem;
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

        public RegulationSelectionViewModel(IRegulationRepository regulationRepository, NavigationService navigationService)
        {
            _regulationRepository = regulationRepository;
            _navigationService = navigationService;
        }

        public async Task InitializeAsync()
        {
            Items.Clear();
            Items.Add(new("Create regulation…", RegulationChoiceKind.Create, null));

            var regulations = await _regulationRepository.GetAllAsync();
            foreach (var r in regulations)
                Items.Add(new(r.Name, RegulationChoiceKind.Existing, r.Id));
        }
    }
}
