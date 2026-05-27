using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class PilotViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _pilotId;

        [ObservableProperty]
        private int _participantId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _kartNumbers = string.Empty;

        [ObservableProperty]
        private string _category = string.Empty;

        [ObservableProperty]
        private string? _photoPath;

        [ObservableProperty]
        private string? _profileUrl;

        public ObservableCollection<string> DynamicData { get; } = [];
    }
}
