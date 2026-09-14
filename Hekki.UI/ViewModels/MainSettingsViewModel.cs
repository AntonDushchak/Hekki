using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class MainSettingsViewModel : ViewModelBase
    {
        private readonly IAppSettingsService _appSettingsService;

        [ObservableProperty]
        private bool? _dialogResult;

        [ObservableProperty]
        private string _selectedLanguage;

        [ObservableProperty]
        private string _selectedTheme;

        [ObservableProperty]
        private string _newTeam = string.Empty;

        [ObservableProperty]
        private string _newLeague = string.Empty;

        [ObservableProperty]
        private string _newKartNumber = string.Empty;

        [ObservableProperty]
        private string? _selectedTeam;

        [ObservableProperty]
        private string? _selectedLeague;

        [ObservableProperty]
        private int? _selectedKartNumber;

        public ObservableCollection<string> Languages { get; } = ["en", "ru"];
        public ObservableCollection<string> Themes { get; } = ["Light", "Dark"];
        public ObservableCollection<int> KartNumbers { get; }
        public ObservableCollection<string> Teams { get; }
        public ObservableCollection<string> Leagues { get; }

        public MainSettingsViewModel(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
            var settings = appSettingsService.Settings;

            SelectedLanguage = settings.Language;
            SelectedTheme = settings.Theme;
            KartNumbers = new ObservableCollection<int>(settings.KartNumbers);
            Teams = new ObservableCollection<string>(settings.Teams);
            Leagues = new ObservableCollection<string>(settings.Leagues);
        }

        [RelayCommand]
        private Task SaveAsync() => ExecuteSafeAsync(async () =>
        {
            var settings = new AppSettings
            {
                Language = SelectedLanguage,
                Theme = SelectedTheme,
                KartNumbers = new ObservableCollection<int>(KartNumbers),
                Teams = new ObservableCollection<string>(Teams),
                Leagues = new ObservableCollection<string>(Leagues)
            };

            await _appSettingsService.ApplyAsync(settings);
            DialogResult = true;
        });

        [RelayCommand]
        private void AddKartNumber()
        {
            if (!int.TryParse(NewKartNumber, out var kartNumber) || kartNumber <= 0 || KartNumbers.Contains(kartNumber))
                return;

            KartNumbers.Add(kartNumber);
            NewKartNumber = string.Empty;
        }

        [RelayCommand]
        private void RemoveKartNumber()
        {
            if (SelectedKartNumber is not int kartNumber)
                return;

            KartNumbers.Remove(kartNumber);
        }

        [RelayCommand]
        private void AddTeam()
        {
            var team = NewTeam.Trim();
            if (string.IsNullOrEmpty(team) || Teams.Contains(team, StringComparer.OrdinalIgnoreCase))
                return;

            Teams.Add(team);
            NewTeam = string.Empty;
        }

        [RelayCommand]
        private void RemoveTeam()
        {
            if (SelectedTeam is null)
                return;

            Teams.Remove(SelectedTeam);
        }

        [RelayCommand]
        private void AddLeague()
        {
            var league = NewLeague.Trim();
            if (string.IsNullOrEmpty(league) || Leagues.Contains(league, StringComparer.OrdinalIgnoreCase))
                return;

            Leagues.Add(league);
            NewLeague = string.Empty;
        }

        [RelayCommand]
        private void RemoveLeague()
        {
            if (SelectedLeague is null)
                return;

            Leagues.Remove(SelectedLeague);
        }

        [RelayCommand]
        private void Cancel() => DialogResult = false;
    }
}
