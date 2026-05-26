using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceParticipantViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _raceId;

        [ObservableProperty]
        private int _pilotId;

        [ObservableProperty]
        private string _pilotName = string.Empty;

        [ObservableProperty]
        private string? _team;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private string? _pilotPhotoPath;

        [ObservableProperty]
        private string? _pilotProfileUrl;
    }
}
