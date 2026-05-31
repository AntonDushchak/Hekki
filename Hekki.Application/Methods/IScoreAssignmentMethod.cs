using Hekki.Application.DTOs;
using System.Text.Json.Serialization;

namespace Hekki.Application.Methods
{
    [JsonDerivedType(typeof(DefaultScoreAssignment), "default")]
    public interface IScoreAssignmentMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        List<PilotDto> AssignScores(List<PilotDto> participants, List<int> scores);
    }

    public interface IScoreAssignmentCatalog
    {
        IReadOnlyList<IScoreAssignmentMethod> GetAll();
        IScoreAssignmentMethod GetById(string id);
    }

    public class ScoreAssignmentCatalog : IScoreAssignmentCatalog
    {
        private readonly IReadOnlyList<IScoreAssignmentMethod> _all;

        private readonly IReadOnlyDictionary<string, IScoreAssignmentMethod> _byId;

        public ScoreAssignmentCatalog(IEnumerable<IScoreAssignmentMethod> methods)
        {
            _all = methods
                .OrderBy(m => m.Title)
                .ToList();

            _byId = _all.ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<IScoreAssignmentMethod> GetAll() => _all;
        public IScoreAssignmentMethod GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Score assignment method id is null/empty.", nameof(id));
            if (_byId.TryGetValue(id, out var method))
                return method;

            throw new KeyNotFoundException($"Unknown score assignment method id: '{id}'.");
        }
    }
}
