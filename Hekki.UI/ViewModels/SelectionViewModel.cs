using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class SelectionViewModel : ObservableObject
    {
        private readonly IRegulationService _regulationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isLoading;
        public ObservableCollection<Regulation> Regulations { get; } = [];
        public IPaginationService PaginationService { get; }
        public ObservableCollection<Regulation> PagedRegulations
        {
            get
            {
                var start = Math.Max(0, PaginationService.StartItem - 1);
                return new ObservableCollection<Regulation>(Regulations.Skip(start).Take(PaginationService.PageCapacity));
            }
        }

        public SelectionViewModel(
            INavigationService navigationService,
            IRegulationService regulationService,
            IPaginationService paginationService)
        {
            _navigationService = navigationService;
            _regulationService = regulationService;
            PaginationService = paginationService;

            Regulations.CollectionChanged += Regulations_CollectionChanged;
            PaginationService.PropertyChanged += PaginationService_PropertyChanged;
        }
        
        public async Task InitializeAsync()
        {
            await LoadRegulationsAsync();
        }

        private void Regulations_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PagedRegulations));
        }

        private void PaginationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PagedRegulations));
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
            _navigationService.NavigateToCreateRace();
        }

        [RelayCommand]
        private void NavigateToRace(Regulation regulation)
        {
            _navigationService.NavigateToRace(regulation.Id);
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