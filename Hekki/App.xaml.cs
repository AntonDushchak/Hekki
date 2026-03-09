using System.IO;
using System.Reflection;
using System.Windows;
using Squirrel;

namespace Hekki
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onEveryRun: OnAppRun);

            if (IsSquirrelInstalled())
                await UpdateMyApp();
        }

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
            if (firstRun) MessageBox.Show("Thanks for installing my application!");
        }

        private static async Task UpdateMyApp()
        {
            using var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/AntonDushchak/Hekki");
            var newVersion = await mgr.UpdateApp();

            if (newVersion != null)
            {
                MessageBox.Show("Доступна новая обновление. Программа перезапустится.");
                UpdateManager.RestartApp();
            }
        }

        private static bool IsSquirrelInstalled()
        {
            var updateExe = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "..", "Update.exe");

            return File.Exists(updateExe);
        }
    }
}
