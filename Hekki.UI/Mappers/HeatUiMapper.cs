using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public static class HeatUiMapper
    {
        public static HeatViewModel MapToHeatViewModel(HeatDto heat)
        {
            var heatViewModel = new HeatViewModel
            {
                Name = heat.Name,
                HeatNumber = heat.HeatNumber,
                ScoringMode = heat.ScoringMode,
            };

            foreach (var groupDto in heat.Groups)
            {
                var groupVm = MapToHeatGroupViewModel(groupDto, heatViewModel);
                heatViewModel.Groups.Add(groupVm);
            }

            return heatViewModel;
        }

        private static HeatGroupViewModel MapToHeatGroupViewModel(HeatGroupDto groupDto, HeatViewModel heatViewModel)
        {
            var groupVm = new HeatGroupViewModel(heatViewModel)
            {
                GroupNumber = groupDto.GroupNumber,
                GroupCapacity = groupDto.GroupCapacity,
                GroupIndex = groupDto.GroupIndex
            };

            foreach (var entryDto in groupDto.Entries)
            {
                var entryVm = ToRow(entryDto, groupDto.Results.FirstOrDefault(r => r.ParticipantId == entryDto.ParticipantId));
                groupVm.Rows.Add(entryVm);
            }

            return groupVm;
        }

        private static HeatRowViewModel ToRow(HeatEntryDto entry, HeatResultDto? result) => new()
        {
            Entry = new HeatEntryViewModel
            {
                ParticipantId = entry.ParticipantId,
                PilotName = entry.PilotName,
                KartNumber = entry.KartNumber,
                GridPosition = entry.GridPosition
            },
            Result = result == null ? null : new HeatResultViewModel
            {
                ParticipantId = result.ParticipantId,
                FinishPosition = result.FinishPosition,
                TotalTimeMs = result.TotalTimeMs,
                Score = result.Score
            }
        };
    }
}
