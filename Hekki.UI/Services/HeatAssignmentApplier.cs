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

                for (var i = 0; i < groupVm.Rows.Count; i++)
                {
                    var row = groupVm.Rows[i];

                    if (i < groupResult.UpdatedEntries.Count)
                    {
                        var entry = groupResult.UpdatedEntries[i];

                        if (row.Entry is null)
                            row.Entry = HeatUiMapper.CreateEntry(entry);
                        else
                            HeatUiMapper.ApplyEntryAssignmentTo(entry, row.Entry);
                    }
                    else
                    {
                        row.Entry = null;
                        row.Result = null;
                    }
                }
            }
        }
    }
}
