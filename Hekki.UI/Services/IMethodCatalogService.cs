using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public interface IMethodCatalogService
    {
        IReadOnlyList<MethodOption> GetShuffleOptions();
        IReadOnlyList<MethodOption> GetGroupOptions();
        IReadOnlyList<MethodOption> GetKartOptions();
        IReadOnlyList<MethodOption> GetScoreOptions();
        MethodOption? FindById(string id);
    }
}