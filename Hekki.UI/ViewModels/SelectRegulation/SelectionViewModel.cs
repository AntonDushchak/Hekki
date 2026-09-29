using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Abstractions;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class SelectionViewModel : ViewModelBase
    {
        private const int RecentRacesMonths = 3;

        private readonly IRegulationService _regulationService;
        private readonly INavigationService _navigationService;
        private readonly IRaceService _raceService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private bool _isLoading;
        public ObservableCollection<RegulationSummaryDto> Regulations { get; } = [];
        public IPaginationService PaginationService { get; }
        public ObservableCollection<RegulationSummaryDto> PagedRegulations
        {
            get
            {
                var start = Math.Max(0, PaginationService.StartItem - 1);
                return new ObservableCollection<RegulationSummaryDto>(Regulations.Skip(start).Take(PaginationService.PageCapacity));
            }
        }

        public SelectionViewModel(
            INavigationService navigationService,
            IRegulationService regulationService,
            IPaginationService paginationService,
            IRaceService raceService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _regulationService = regulationService;
            _raceService = raceService;
            _dialogService = dialogService;
            PaginationService = paginationService;

            Regulations.CollectionChanged += Regulations_CollectionChanged;
            PaginationService.PropertyChanged += PaginationService_PropertyChanged;
        }

        public Task InitializeAsync() => ExecuteSafeAsync(async () =>
        {
            await LoadRegulationsAsync();
        });

        private void Regulations_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PagedRegulations));
        }

        private void PaginationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PagedRegulations));
        }

        private Task LoadRegulationsAsync() => ExecuteSafeAsync(async () =>
        {
            try
            {
                IsLoading = true;
                Regulations.Clear();

                var regs = await _regulationService.GetRegulationsAsync();

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
        });

        [RelayCommand]
        private void NavigateToCreation()
        {
            _navigationService.NavigateToCreateRace();
        }

        [RelayCommand]
        private Task LoadRaceAsync() => ExecuteSafeAsync(async () =>
        {
            var races = await _raceService.GetRacesSinceAsync(DateTime.UtcNow.AddMonths(-RecentRacesMonths));

            var race = _dialogService.ShowLoadRace(new LoadRaceViewModel(races));
            if (race == null) return;

            await _navigationService.NavigateToRace(race.RegulationId, race.RaceId);
        });

        [RelayCommand]
        private void NavigateToRace(RegulationSummaryDto regulation)
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

        [RelayCommand]
        private Task DeleteRegulation(RegulationSummaryDto dto) => ExecuteSafeAsync(async () =>
        {
            await _regulationService.DeleteRegulationAsync(dto.Id);

            Regulations.Remove(dto);

            PaginationService.SetTotalItems(Regulations.Count);
        });
    }
}