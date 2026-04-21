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
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        private bool _isLoading;
        public ObservableCollection<Regulation> Regulations { get; } = [];
        public IPaginationService PaginationService { get; }



        public SelectionViewModel(
            NavigationService navigationService,
            IRegulationService regulationService,
            IViewModelFactory viewModelFactory,
            IPaginationService paginationService)
        {
            _navigationService = navigationService;
            _regulationService = regulationService;
            _viewModelFactory = viewModelFactory;
            PaginationService = paginationService;
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

                PaginationService.SetTotalItems(Regulations.Count);
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

        [RelayCommand]
        private void Prev()
        {
            PaginationService.Prev();
        }

        [RelayCommand]
        private void Next()
        {
            PaginationService.Next();
        }
    }
}