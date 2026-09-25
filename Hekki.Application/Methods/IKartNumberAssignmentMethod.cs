using Hekki.Application.Models;
using System.Text.Json.Serialization;

namespace Hekki.Application.Methods
{
    [JsonDerivedType(typeof(RandomKartAssignment), "random_kart_assignment")]
    [JsonDerivedType(typeof(RandomNoRepeatKartAssignment), "random_no_repeat_kart_assignment")]
    public interface IKartNumberAssignmentMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        void AssignKartNumber(
            IReadOnlyList<ParticipantAssignment> assignments,
            IReadOnlyDictionary<Guid, IReadOnlyList<int>> previousKartNumbers,
            IReadOnlyList<int> availableKarts);
    }

    public interface IKartNumberAssignmentCatalog
    {
        IReadOnlyList<IKartNumberAssignmentMethod> GetAll();
        IKartNumberAssignmentMethod GetById(string id);
    }

    public class KartNumberAssignmentCatalog : IKartNumberAssignmentCatalog
    {
        private readonly IReadOnlyList<IKartNumberAssignmentMethod> _all;

        public KartNumberAssignmentCatalog(IEnumerable<IKartNumberAssignmentMethod> methods)
        {
            _all = methods
                .OrderBy(m => m.Title)
                .ToList();

            _byId = _all.ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);
        }

        private readonly IReadOnlyDictionary<string, IKartNumberAssignmentMethod> _byId;


        public IReadOnlyList<IKartNumberAssignmentMethod> GetAll() => _all;
        public IKartNumberAssignmentMethod GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Kart number assignment method id is null/empty.", nameof(id));

            if (_byId.TryGetValue(id, out var method))
                return method;

            throw new KeyNotFoundException($"Unknown kart number assignment method id: '{id}'.");
        }
    }
}
