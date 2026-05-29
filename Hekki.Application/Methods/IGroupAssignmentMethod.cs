using Hekki.Application.DTOs;

namespace Hekki.Application.Methods
{
    public interface IGroupAssignmentMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        List<List<PilotDto>> AssignGroups(List<PilotDto> participants, int groupSize, int groupCount);
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
