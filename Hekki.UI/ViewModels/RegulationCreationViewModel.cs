using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RegulationCreationViewModel : ObservableObject
    {
        [ObservableProperty]
        private HeatConfiguration? _selectedHeat;

        public ObservableCollection<HeatConfiguration> Heats { get; } = [];

        public List<string> AvailableShuffleMethods { get; } = new() { "Random", "By Rank", "None" };
        public List<string> AvailableGroupMethods { get; } = new() { "Balanced", "Sequential" };
        public List<string> AvailableKartMethods { get; } = new() { "Random", "By Rank", "None" };
        public List<string> AvailableScoreMethods { get; } = new() { "Balanced", "Sequential" };

        public RegulationCreationViewModel()
        {
            Heats.Add(new HeatConfiguration { Name = "Заезд 1" });
            Heats.Add(new HeatConfiguration { Name = "Заезд 2" });
        }
    }
}
