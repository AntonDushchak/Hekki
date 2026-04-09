using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.Methods;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RegulationCreationViewModel : ObservableObject
    {
        private readonly IParticipantShuffleCatalog _shuffleCatalog;
        private readonly IGroupAssignmentCatalog _groupCatalog;
        private readonly IKartNummerAssignmentCatalog _kartCatalog;
        private readonly IScoreAssignmentCatalog _scoreCatalog;

        [ObservableProperty]
        private HeatConfiguration? _selectedHeat;

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
            IScoreAssignmentCatalog scoreCatalog)
        {
            _shuffleCatalog = shuffleCatalog;
            _groupCatalog = groupCatalog;
            _kartCatalog = kartCatalog;
            _scoreCatalog = scoreCatalog;

            Fill(AvailableShuffleMethods, _shuffleCatalog.GetAll());
            Fill(AvailableGroupMethods, _groupCatalog.GetAll());
            Fill(AvailableKartMethods, _kartCatalog.GetAll());
            Fill(AvailableScoreMethods, _scoreCatalog.GetAll());

            _selectedShuffleMethodId = AvailableShuffleMethods.FirstOrDefault()?.Id;
            _selectedGroupMethodId = AvailableGroupMethods.FirstOrDefault()?.Id;
            _selectedKartMethodId = AvailableKartMethods.FirstOrDefault()?.Id;
            _selectedScoreMethodId = AvailableScoreMethods.FirstOrDefault()?.Id;

            Heats.Add(new HeatConfiguration { Name = "Заезд 1" });
            Heats.Add(new HeatConfiguration { Name = "Заезд 2" });
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
    }
}
