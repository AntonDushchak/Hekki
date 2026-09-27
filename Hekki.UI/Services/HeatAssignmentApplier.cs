using Hekki.Application.DTOs.Race;
using Hekki.UI.Mappers;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public static class HeatAssignmentApplier
    {
        public static void Apply(IEnumerable<HeatViewModel> heats, IReadOnlyList<GroupAssignmentResultDto> results)
        {
            foreach (var groupResult in results)
            {
                var groupVm = heats
                    .SelectMany(heat => heat.Groups)
                    .FirstOrDefault(group =>
                        group.GroupId == groupResult.Group.Id);

                if (groupVm is null)
                    continue;

                var previousRows = groupVm.Rows
                    .Where(row => row.HasParticipant)
                    .ToDictionary(row => row.Entry!.ParticipantId);

                groupVm.Rows.Clear();

                foreach (var entry in groupResult.UpdatedEntries)
                {
                    if (previousRows.TryGetValue(entry.ParticipantId, out var row))
                        HeatUiMapper.ApplyEntryAssignmentTo(entry, row.Entry!);
                    else
                        row = new HeatRowViewModel { Entry = HeatUiMapper.CreateEntry(entry) };

                    groupVm.Rows.Add(row);
                }

                groupVm.RefreshCells();
            }
        }
    }
}
