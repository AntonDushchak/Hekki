using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class RaceParticipantViewModel : ObservableObject
    {
        [ObservableProperty] private Guid _id;
        [ObservableProperty] private int _raceId;
        [ObservableProperty] private int _pilotId;
        [ObservableProperty] private string _firstName = string.Empty;
        [ObservableProperty] private string _lastName = string.Empty;
        [ObservableProperty] private string? _team;
        [ObservableProperty] private string? _league;
        [ObservableProperty] private bool _isActive = true;
        [ObservableProperty] private string? _pilotPhotoPath;
        [ObservableProperty] private string? _pilotProfileUrl;
        private ObservableCollection<int> _kartNumbers = [];
        public string KartNumbersDisplay => string.Join(", ", _kartNumbers);
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
