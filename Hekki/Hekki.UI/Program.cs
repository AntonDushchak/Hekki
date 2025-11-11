using Squirrel;
using System.Reflection;
using System.Threading.Tasks;

namespace Hekki.Hekki.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onEveryRun: OnAppRun);

            if (IsSquirrelInstalled())
                await UpdateMyApp();

            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
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
            using var mgr = UpdateManager.GitHubUpdateManager("https://github.com/AntonDushchak/Hekki");
            var newVersion = await mgr.Result.UpdateApp();

            if (newVersion != null)
            {
                MessageBox.Show("Доступно новое обновление. Сейчас перезапутим программу.");
                UpdateManager.RestartApp();
            }
        }

        private static bool IsSquirrelInstalled()
        {
            var updateExe = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "..", "Update.exe");

            return File.Exists(Path.GetFullPath(updateExe));
        }

    }
}