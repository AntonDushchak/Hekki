using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RegulationCreationViewModel : ObservableObject
    {
        private string _regulationName = string.Empty;
        private readonly IParticipantShuffleCatalog _shuffleCatalog;
        private readonly IGroupAssignmentCatalog _groupCatalog;
        private readonly IKartNummerAssignmentCatalog _kartCatalog;
        private readonly IScoreAssignmentCatalog _scoreCatalog;
        private readonly IServiceScopeFactory _scopeFactory;
        [ObservableProperty]
        private HeatConfiguration? _selectedHeat;

        public string RegulationName
        {
            get => _regulationName;
            set => SetProperty(ref _regulationName, value);
        }
        public ObservableCollection<HeatConfiguration> Heats { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableShuffleMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableGroupMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableKartMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableScoreMethods { get; } = [];


        [ObservableProperty] private string? _selectedShuffleMethodId;
        [ObservableProperty] private string? _selectedGroupMethodId;
        [ObservableProperty] private string? _selectedKartMethodId;
        [ObservableProperty] private string? _selectedScoreMethodId;


        public RegulationCreationViewModel(
            IParticipantShuffleCatalog shuffleCatalog,
            IGroupAssignmentCatalog groupCatalog,
            IKartNummerAssignmentCatalog kartCatalog,
            IScoreAssignmentCatalog scoreCatalog,
            IServiceScopeFactory scopeFactory)
        {
            _shuffleCatalog = shuffleCatalog;
            _groupCatalog = groupCatalog;
            _kartCatalog = kartCatalog;
            _scoreCatalog = scoreCatalog;
            _scopeFactory = scopeFactory;

            Fill(AvailableShuffleMethods, _shuffleCatalog.GetAll());
            Fill(AvailableGroupMethods, _groupCatalog.GetAll());
            Fill(AvailableKartMethods, _kartCatalog.GetAll());
            Fill(AvailableScoreMethods, _scoreCatalog.GetAll());

            _selectedShuffleMethodId = AvailableShuffleMethods.FirstOrDefault()?.Id;
            _selectedGroupMethodId = AvailableGroupMethods.FirstOrDefault()?.Id;
            _selectedKartMethodId = AvailableKartMethods.FirstOrDefault()?.Id;
            _selectedScoreMethodId = AvailableScoreMethods.FirstOrDefault()?.Id;

        }

        private static void Fill<TMethod>(
            ObservableCollection<MethodOptionViewModel> target,
            IReadOnlyList<TMethod> methods)
            where TMethod : class
        {
            target.Clear();

            foreach (dynamic m in methods)
                target.Add(new MethodOptionViewModel(m.Id, m.Title, m.Description));
        }

        [RelayCommand]
        private void AddHeat()
        {
            int nextNumber = Heats.Count + 1;
            Heats.Add(new HeatConfiguration { Name = $"Heat {nextNumber}" });
        }

        [RelayCommand]
        private async Task Save()
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRegulationRepository>();
            try
            {
                await repo.AddAsync(RegulationUiMapper.ToDomain(this, 0, 1));
            }
            catch 
            {
                //TODO: Handle error
            }

        }
    }
}
