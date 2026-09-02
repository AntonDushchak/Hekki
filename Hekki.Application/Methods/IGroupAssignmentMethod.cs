using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.Application.Models;
using System.Text.Json.Serialization;

namespace Hekki.Application.Methods
{
    [JsonDerivedType(typeof(RandomGroupAssignment), "random_group_assignment")]
    [JsonDerivedType(typeof(CardGroupAssignment), "card_group_assignment")]
    [JsonDerivedType(typeof(ListGroupAssignment), "list_group_assignment")]
    [JsonDerivedType(typeof(ReplacementGroupAssignment), "replacement_group_assignment")]
    public interface IGroupAssignmentMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        List<List<ParticipantAssignment>> AssignGroups(List<RaceParticipantDto> participants, int groupSize, int groupCount);
    }

    public interface IGroupAssignmentCatalog
    {
        IReadOnlyList<IGroupAssignmentMethod> GetAll();
        IGroupAssignmentMethod GetById(string id);
    }

    public class GroupAssigmentCatalog : IGroupAssignmentCatalog
    {
        private readonly IReadOnlyList<IGroupAssignmentMethod> _all;

        private readonly IReadOnlyDictionary<string, IGroupAssignmentMethod> _byId;
        public GroupAssigmentCatalog(IEnumerable<IGroupAssignmentMethod> methods)
        {
            _all = methods
                .OrderBy(m => m.Title)
                .ToList();

            _byId = _all.ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);
        }


        public IReadOnlyList<IGroupAssignmentMethod> GetAll() => _all;

        public IGroupAssignmentMethod GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Group assignment method id is null/empty.", nameof(id));

            if (_byId.TryGetValue(id, out var method))
                return method;

            throw new KeyNotFoundException($"Unknown group assignment method id: '{id}'.");
        }
    }
}
