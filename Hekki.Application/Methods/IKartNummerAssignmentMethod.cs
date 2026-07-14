using Hekki.Application.DTOs.Pilot;
using System.Text.Json.Serialization;

namespace Hekki.Application.Methods
{
    [JsonDerivedType(typeof(RandomKartAssignment), "random_kart_assignment")]
    [JsonDerivedType(typeof(RandomNoRepeatKartAssignment), "random_no_repeat_kart_assignment")]
    public interface IKartNummerAssignmentMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        List<PilotDto> AssignKartNummer(List<PilotDto> participants, List<int> kartNummers);
    }

    public interface IKartNummerAssignmentCatalog
    {
        IReadOnlyList<IKartNummerAssignmentMethod> GetAll();
        IKartNummerAssignmentMethod GetById(string id);
    }

    public class KartNummerAssigmentCatalog : IKartNummerAssignmentCatalog
    {
        private readonly IReadOnlyList<IKartNummerAssignmentMethod> _all;

        public KartNummerAssigmentCatalog(IEnumerable<IKartNummerAssignmentMethod> methods)
        {
            _all = methods
                .OrderBy(m => m.Title)
                .ToList();

            _byId = _all.ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);
        }

        private readonly IReadOnlyDictionary<string, IKartNummerAssignmentMethod> _byId;


        public IReadOnlyList<IKartNummerAssignmentMethod> GetAll() => _all;
        public IKartNummerAssignmentMethod GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Kart nummer assignment method id is null/empty.", nameof(id));

            if (_byId.TryGetValue(id, out var method))
                return method;

            throw new KeyNotFoundException($"Unknown kart nummer assignment method id: '{id}'.");
        }
    }
}
