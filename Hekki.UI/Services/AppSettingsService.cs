using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Hekki.UI.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private const string SettingsFileName = "settings.json";
        private static readonly Uri LightThemeUri = new("Resources/LightTheme.xaml", UriKind.Relative);
        private static readonly Uri DarkThemeUri = new("Resources/DarkTheme.xaml", UriKind.Relative);
        private static readonly Uri EnglishResourcesUri = new("Resources/Resources.en.xaml", UriKind.Relative);
        private static readonly Uri RussianResourcesUri = new("Resources/Resources.ru.xaml", UriKind.Relative);
        private readonly string _settingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Hekki",
            SettingsFileName);

        public AppSettings Settings { get; }

        public AppSettingsService(AppSettings settings)
        {
            Settings = settings;
        }

        public async Task LoadAsync()
        {
            if (!File.Exists(_settingsFilePath))
            {
                ApplyResources(Settings);
                return;
            }

            await using var stream = File.OpenRead(_settingsFilePath);
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream);
            if (settings is null)
            {
                ApplyResources(Settings);
                return;
            }

            CopySettings(settings, Settings);
            ApplyResources(Settings);
        }

        public async Task ApplyAsync(AppSettings settings)
        {
            CopySettings(settings, Settings);
            ApplyResources(Settings);

            var directory = Path.GetDirectoryName(_settingsFilePath)!;
            Directory.CreateDirectory(directory);
            await using var stream = File.Create(_settingsFilePath);
            await JsonSerializer.SerializeAsync(stream, Settings, new JsonSerializerOptions { WriteIndented = true });
        }

        private static void CopySettings(AppSettings source, AppSettings target)
        {
            target.Language = source.Language;
            target.Theme = source.Theme;
            target.KartNumbers = new ObservableCollection<int>(source.KartNumbers);
            target.Teams = new ObservableCollection<string>(source.Teams);
            target.Leagues = new ObservableCollection<string>(source.Leagues);
            target.Locations = new ObservableCollection<string>(source.Locations);
        }

        private static void ApplyResources(AppSettings settings)
        {
            var dictionaries = System.Windows.Application.Current.Resources.MergedDictionaries;
            ReplaceDictionary(dictionaries, LightThemeUri, DarkThemeUri, settings.Theme == "Dark" ? DarkThemeUri : LightThemeUri);
            ReplaceDictionary(dictionaries, EnglishResourcesUri, RussianResourcesUri, settings.Language == "ru" ? RussianResourcesUri : EnglishResourcesUri);
        }

        private static void ReplaceDictionary(
            Collection<ResourceDictionary> dictionaries,
            Uri firstUri,
            Uri secondUri,
            Uri replacementUri)
        {
            var existing = dictionaries.FirstOrDefault(dictionary =>
                dictionary.Source == firstUri || dictionary.Source == secondUri);

            var replacement = new ResourceDictionary { Source = replacementUri };
            if (existing is null)
                dictionaries.Add(replacement);
            else
                dictionaries[dictionaries.IndexOf(existing)] = replacement;
        }
    }
}
