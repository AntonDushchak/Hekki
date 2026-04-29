using Hekki.Application.Methods;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public class MethodCatalogService : IMethodCatalogService
    {
        private readonly IParticipantShuffleCatalog _shuffle;
        private readonly IGroupAssignmentCatalog _group;
        private readonly IKartNummerAssignmentCatalog _kart;
        private readonly IScoreAssignmentCatalog _score;

        private readonly Lazy<IReadOnlyList<MethodOptionViewModel>> _shuffleOptions;
        private readonly Lazy<IReadOnlyList<MethodOptionViewModel>> _groupOptions;
        private readonly Lazy<IReadOnlyList<MethodOptionViewModel>> _kartOptions;
        private readonly Lazy<IReadOnlyList<MethodOptionViewModel>> _scoreOptions;

        public MethodCatalogService(
            IParticipantShuffleCatalog shuffle,
            IGroupAssignmentCatalog group,
            IKartNummerAssignmentCatalog kart,
            IScoreAssignmentCatalog score)
        {
            _shuffle = shuffle;
            _group = group;
            _kart = kart;
            _score = score;

            _shuffleOptions = new Lazy<IReadOnlyList<MethodOptionViewModel>>(() => Map(_shuffle.GetAll()));
            _groupOptions = new Lazy<IReadOnlyList<MethodOptionViewModel>>(() => Map(_group.GetAll()));
            _kartOptions = new Lazy<IReadOnlyList<MethodOptionViewModel>>(() => Map(_kart.GetAll()));
            _scoreOptions = new Lazy<IReadOnlyList<MethodOptionViewModel>>(() => Map(_score.GetAll()));
        }

        public IReadOnlyList<MethodOptionViewModel> GetShuffleOptions() => _shuffleOptions.Value;
        public IReadOnlyList<MethodOptionViewModel> GetGroupOptions() => _groupOptions.Value;
        public IReadOnlyList<MethodOptionViewModel> GetKartOptions() => _kartOptions.Value;
        public IReadOnlyList<MethodOptionViewModel> GetScoreOptions() => _scoreOptions.Value;

        public MethodOptionViewModel? FindById(string id)
        {
            return _shuffleOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _groupOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _kartOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _scoreOptions.Value.FirstOrDefault(x => x.Id == id);
        }

        private static IReadOnlyList<MethodOptionViewModel> Map<T>(IReadOnlyList<T> src)
        {
            var list = new List<MethodOptionViewModel>(src.Count);
            foreach (dynamic m in src)
                list.Add(new MethodOptionViewModel(m.Id, m.Title, m.Description));
            return list;
        }
    }
}
