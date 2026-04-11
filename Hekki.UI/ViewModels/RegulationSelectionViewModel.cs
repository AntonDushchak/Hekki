using Hekki.Application.Abstrations;
using Hekki.UI.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hekki.UI.ViewModels
{
    public class RegulationSelectionViewModel : INotifyPropertyChanged
    {
        private readonly IServiceScopeFactory _scopeFactory;
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

        public RegulationSelectionViewModel(IServiceScopeFactory scopeFactory, NavigationService navigationService)
        {
            _scopeFactory = scopeFactory;
            _navigationService = navigationService;
        }

        public void NavigateToSelected()
        {
            if (SelectedItem is null) return;

            if (SelectedItem.Kind == RegulationChoiceKind.Create)
                _navigationService.Navigate(new Uri("/Pages/RegulationCreation.xaml", UriKind.Relative));
            else
                _navigationService.NavigateToRace(SelectedItem.RegulationId!.Value);
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

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRegulationRepository>();
            var regulations = await repo.GetAllAsync();
            foreach (var r in regulations)
                Items.Add(new(r.Name, RegulationChoiceKind.Existing, r.Id));

            _initialized = true;
        }
    }
}
