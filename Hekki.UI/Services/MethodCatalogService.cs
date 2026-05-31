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

        private readonly Lazy<IReadOnlyList<MethodOption>> _shuffleOptions;
        private readonly Lazy<IReadOnlyList<MethodOption>> _groupOptions;
        private readonly Lazy<IReadOnlyList<MethodOption>> _kartOptions;
        private readonly Lazy<IReadOnlyList<MethodOption>> _scoreOptions;

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

            _shuffleOptions = new Lazy<IReadOnlyList<MethodOption>>(() => Map(_shuffle.GetAll()));
            _groupOptions = new Lazy<IReadOnlyList<MethodOption>>(() => Map(_group.GetAll()));
            _kartOptions = new Lazy<IReadOnlyList<MethodOption>>(() => Map(_kart.GetAll()));
            _scoreOptions = new Lazy<IReadOnlyList<MethodOption>>(() => Map(_score.GetAll()));
        }

        public IReadOnlyList<MethodOption> GetShuffleOptions() => _shuffleOptions.Value;
        public IReadOnlyList<MethodOption> GetGroupOptions() => _groupOptions.Value;
        public IReadOnlyList<MethodOption> GetKartOptions() => _kartOptions.Value;
        public IReadOnlyList<MethodOption> GetScoreOptions() => _scoreOptions.Value;

        public MethodOption? FindById(string id)
        {
            return _shuffleOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _groupOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _kartOptions.Value.FirstOrDefault(x => x.Id == id)
                ?? _scoreOptions.Value.FirstOrDefault(x => x.Id == id);
        }

        public IParticipantShuffleMethod? CreateShuffleMethod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _shuffle.GetById(id);
        }

        public IGroupAssignmentMethod? CreateGroupMethod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _group.GetById(id);
        }

        public IKartNummerAssignmentMethod? CreateKartMethod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _kart.GetById(id);
        }

        public IScoreAssignmentMethod? CreateScoreMethod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _score.GetById(id);
        }

        private static IReadOnlyList<MethodOption> Map<T>(IReadOnlyList<T> src)
        {
            var list = new List<MethodOption>(src.Count);
            foreach (dynamic m in src)
                list.Add(new MethodOption(m.Id, m.Title, m.Description));
            return list;
        }
    }
}
