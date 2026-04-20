using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class SelectionViewModel : ObservableObject
    {
        private readonly IRegulationService _regulationService;
        private readonly IViewModelFactory _viewModelFactory;

        public ObservableCollection<Regulation> Regulations { get; } = [];

        [ObservableProperty]
        private bool _isLoading;
        private readonly NavigationService _navigationService;

        public SelectionViewModel(NavigationService navigationService, IRegulationService regulationService, IViewModelFactory viewModelFactory)
        {
            _navigationService = navigationService;
            _regulationService = regulationService;
            _viewModelFactory = viewModelFactory;
        }

        public async Task InitializeAsync()
        {
            await LoadRegulationsAsync();
        }

        private async Task LoadRegulationsAsync()
        {
            try
            {
                IsLoading = true;
                Regulations.Clear();

                var regs = await _regulationService.GetLookupAsync();

                foreach (var reg in regs)
                {
                    Regulations.Add(reg);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void NavigateToCreation()
        {
            _navigationService.Go(_viewModelFactory.Create<CreateRaceViewModel>());
        }
    }
}