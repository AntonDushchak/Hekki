namespace Hekki.UI.Services
{
    public interface IAppSettingsService
    {
        AppSettings Settings { get; }

        Task LoadAsync();
        Task ApplyAsync(AppSettings settings);
    }
}
