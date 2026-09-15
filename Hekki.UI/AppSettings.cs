using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI
{
    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        private string _language = "en";

        [ObservableProperty]
        private string _theme = "Light";

        [ObservableProperty]
        private ObservableCollection<int> _kartNumbers = new();

        [ObservableProperty]
        private ObservableCollection<string> _teams = new();

        [ObservableProperty]
        private ObservableCollection<string> _leagues = new();

        [ObservableProperty]
        private ObservableCollection<string> _locations = new();
    }
}
