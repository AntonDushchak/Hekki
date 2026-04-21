using Hekki.Domain.Models;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public class RegulationUiMapper
    {
        public static Regulation ToDomain(RegulationCreationViewModel vm, int id, int version, DateTime creationDate)
        => new()
        {
            Id = id,
            Name = vm.RegulationName,
            Version = version,
            CreationDate = creationDate,
            Configurations = vm.Heats.Select(h => new HeatConfigurationModel
            {
                Name = h.Name,
                Shuffle = new MethodSettings { MethodId = h.Shuffle.MethodId, Arguments = new(h.Shuffle.Arguments) },
                Grouping = new MethodSettings { MethodId = h.Grouping.MethodId, Arguments = new(h.Grouping.Arguments) },
                KartAssignment = new MethodSettings { MethodId = h.KartAssignment.MethodId, Arguments = new(h.KartAssignment.Arguments) },
                Scoring = new MethodSettings { MethodId = h.Scoring.MethodId, Arguments = new(h.Scoring.Arguments) },
            }).ToList()
        };
    }
}
