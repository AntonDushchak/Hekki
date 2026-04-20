using Hekki.Application.Abstrations;
using Hekki.UI.Enums;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hekki.UI.ViewModels
{
    public class RegulationPickerViewModel : INotifyPropertyChanged
    {
        private readonly IRegulationRepository _regulationRepository;
        private readonly NavigationService _navigationService;

        private RegulationChoiceItem? _selectedItem;
        private bool _initialized = false;
        public ObservableCollection<RegulationChoiceItem> Items { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        public RegulationChoiceItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged();
                NavigateToSelected();
            }
        }

        public RegulationPickerViewModel(IRegulationRepository regulationRepository, NavigationService navigationService)
        {
            _regulationRepository = regulationRepository;
            _navigationService = navigationService;
        }

        public void NavigateToSelected()
        {
            if (SelectedItem is null) return;

            if (SelectedItem.Kind == RegulationChoiceKind.Create)
                _navigationService.Navigate(new Uri("/Pages/RegulationCreation.xaml", UriKind.Relative));
            else
                _navigationService.Navigate(new Uri($"/Pages/Race.xaml?regulationId={SelectedItem.RegulationId}", UriKind.Relative));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            Items.Clear();
            Items.Add(new("Create regulation…", RegulationChoiceKind.Create, null));

            var regulations = await _regulationRepository.GetAllAsync();
            foreach (var r in regulations)
                Items.Add(new(r.Name, RegulationChoiceKind.Existing, r.Id));

            _initialized = true;
        }
    }
}
