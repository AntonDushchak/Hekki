using Hekki.Application.Methods;
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

        IParticipantShuffleMethod? CreateShuffleMethod(string id);
        IGroupAssignmentMethod? CreateGroupMethod(string id);
        IKartNummerAssignmentMethod? CreateKartMethod(string id);
        IScoreAssignmentMethod? CreateScoreMethod(string id);
    }
}