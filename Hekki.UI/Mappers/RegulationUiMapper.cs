using Hekki.Domain.Models;
using Hekki.UI.ViewModels;
using System.Text.Json;

namespace Hekki.UI.Mappers
{
    public class RegulationUiMapper
    {
        public static Regulation ToDomain(CreateRegulationViewModel vm, int id, int version, DateTime creationDate)
        => new()
        {
            Id = id,
            Name = vm.RegulationName,
            Version = version,
            CreationDate = DateTime.SpecifyKind(creationDate, DateTimeKind.Utc),
            Configurations = vm.Heats.Select(h => new HeatConfigurationModel
            {
                Name = h.Name,
                Shuffle = new MethodSettings
                {
                    MethodId = h.ShuffleMethodId,
                    Arguments = ConvertParametersToArguments(h.ShuffleParameters)
                },
                Grouping = new MethodSettings
                {
                    MethodId = h.GroupMethodId,
                    Arguments = ConvertParametersToArguments(h.GroupParameters)
                },
                KartAssignment = new MethodSettings
                {
                    MethodId = h.KartMethodId,
                    Arguments = ConvertParametersToArguments(h.KartParameters)
                },
                Scoring = new MethodSettings
                {
                    MethodId = h.ScoreMethodId,
                    Arguments = ConvertParametersToArguments(h.ScoreParameters)
                },
            }).ToList()
        };

        private static Dictionary<string, JsonElement> ConvertParametersToArguments(MethodParameters? parameters)
        {
            if (parameters == null)
                return [];

            return parameters switch
            {
                ReplacementParameters replacement => new()
                {
                    ["numberToDown"] = JsonSerializer.SerializeToElement(replacement.NumberToDown),
                    ["numberToUp"] = JsonSerializer.SerializeToElement(replacement.NumberToUp)
                },

                _ => []
            };
        }

        public static CreateRegulationViewModel FromDomain(Regulation regulation)
        {
            // TODO: Implement reverse mapping
            throw new NotImplementedException();
        }
    }
}
