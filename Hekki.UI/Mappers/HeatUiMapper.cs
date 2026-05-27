using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Domain.Models;
using Hekki.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Hekki.UI.Mappers
{
    public static class HeatUiMapper
    {
        /// <summary>
        /// Map Heat domain model to HeatViewModel for UI display
        /// </summary>
        public static HeatViewModel MapToHeatViewModel(
            Heat heat, 
            HeatConfigurationModel config,
            IReadOnlyList<HeatEntry> entries,
            IReadOnlyList<HeatParticipantResult> results,
            IReadOnlyList<PilotDto> participants)
        {
            var heatViewModel = new HeatViewModel
            {
                Name = heat.Name,
                HeatNumber = heat.ConfigurationIndex + 1
            };

            var groups = MapToHeatGroups(config, entries, results, participants);
            foreach (var group in groups)
            {
                heatViewModel.Groups.Add(group);
            }

            return heatViewModel;
        }

        /// <summary>
        /// Create groups of participants based on heat configuration
        /// </summary>
        private static IEnumerable<HeatGroupViewModel> MapToHeatGroups(
            HeatConfigurationModel config,
            IReadOnlyList<HeatEntry> entries,
            IReadOnlyList<HeatParticipantResult> results,
            IReadOnlyList<PilotDto> participants)
        {
            var numberOfGroups = config.NumberOfGroups > 0 ? config.NumberOfGroups : 1;
            var groupCapacity = config.GroupCapacity > 0 ? config.GroupCapacity : 8;

            for (int groupNum = 1; groupNum <= numberOfGroups; groupNum++)
            {
                var group = new HeatGroupViewModel
                {
                    GroupNumber = groupNum,
                    GroupCapacity = groupCapacity
                };

                var groupEntries = entries
                    .OrderBy(e => e.SeedOrder)
                    .Skip((groupNum - 1) * groupCapacity)
                    .Take(groupCapacity)
                    .ToList();

                foreach (var entry in groupEntries)
                {
                    var participant = participants.FirstOrDefault(p => p.ParticipantId == entry.ParticipantId);
                    var result = results.FirstOrDefault(r => r.ParticipantId == entry.ParticipantId);

                    if (participant != null)
                    {
                        var resultVm = new HeatResultViewModel
                        {
                            Position = result?.FinishPosition ?? 0,
                            KartNumber = entry.KartNumber?.ToString() ?? "-",
                            PilotName = participant.Name
                        };

                        // Add dynamic data if needed (laps, times, etc.)
                        if (result != null)
                        {
                            resultVm.DynamicData.Add(result.Laps?.ToString() ?? "-");
                            resultVm.DynamicData.Add(FormatTime(result.BestLapMs));
                            resultVm.DynamicData.Add(FormatTime(result.TotalTimeMs));
                        }

                        group.Results.Add(resultVm);
                    }
                }

                yield return group;
            }
        }

        /// <summary>
        /// Format time in milliseconds to readable format (MM:SS.mmm)
        /// </summary>
        private static string FormatTime(long? timeMs)
        {
            if (timeMs == null || timeMs == 0)
                return "-";

            var totalSeconds = timeMs.Value / 1000.0;
            var minutes = (int)(totalSeconds / 60);
            var seconds = totalSeconds % 60;

            return $"{minutes:D2}:{seconds:00.000}";
        }
    }
}
