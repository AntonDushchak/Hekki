using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public interface IMethodCatalogService
    {
        IReadOnlyList<MethodOptionViewModel> GetShuffleOptions();
        IReadOnlyList<MethodOptionViewModel> GetGroupOptions();
        IReadOnlyList<MethodOptionViewModel> GetKartOptions();
        IReadOnlyList<MethodOptionViewModel> GetScoreOptions();
        MethodOptionViewModel? FindById(string id);
    }
}