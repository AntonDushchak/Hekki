using Hekki.Application.DTOs;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public static class HeatUiMapper
    {
        /// <summary>
        /// Map Heat domain model to HeatViewModel for UI display
        /// </summary>
        public static HeatViewModel MapToHeatViewModel(HeatDto heat)
        {
            var heatViewModel = new HeatViewModel
            {
                Name = heat.Name,
                HeatNumber = heat.HeatNumber,
                ScoringMode = heat.ScoringMode
            };

            foreach (var groupDto in heat.Groups)
            {
                var groupVm = MapToHeatGroupViewModel(groupDto);
                heatViewModel.Groups.Add(groupVm);
            }

            return heatViewModel;
        }

        /// <summary>
        /// Map HeatGroupDto to HeatGroupViewModel
        /// </summary>
        private static HeatGroupViewModel MapToHeatGroupViewModel(HeatGroupDto groupDto)
        {
            var groupVm = new HeatGroupViewModel
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

        public static HeatRowViewModel ToRow(HeatEntryDto entry, HeatResultDto? result) => new()
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
