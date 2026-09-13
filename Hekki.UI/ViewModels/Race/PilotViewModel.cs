using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class PilotViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _pilotId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string _lastName = string.Empty;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
