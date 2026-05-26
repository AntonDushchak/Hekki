using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class PilotViewModel : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _kartNumbers = string.Empty;

        [ObservableProperty]
        private string _category = string.Empty;

        [ObservableProperty]
        private string? _photoPath;

        public ObservableCollection<string> DynamicData { get; } = [];
    }
}
