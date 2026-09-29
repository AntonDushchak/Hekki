using Hekki.Application.DTOs.Regulation;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public static class HeatConfigUiMapper
    {
        public static HeatConfigurationViewModel ToViewModel(HeatConfig config) => new()
        {
            Name = config.Name,
            HeatNumber = config.HeatNumber,
            NumberOfGroups = config.GroupCount,
            GroupCapacity = config.ParticipantsPerGroup,
            UsePoints = config.ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid,
            UseTime = config.ScoringMode is ScoringMode.TimeBased or ScoringMode.Hybrid,
            UsePenalty = config.Scoring.UsePenalties,
            ShuffleMethodId = config.Assignment.Shuffle?.Id ?? string.Empty,
            GroupMethodId = config.Assignment.GroupMethod?.Id ?? string.Empty,
            KartMethodId = config.Assignment.KartMethod?.Id ?? string.Empty,
            ScoreMethodId = config.Scoring.Method?.Id ?? string.Empty
        };

        public static HeatConfig ToConfig(HeatConfigurationViewModel heat, IMethodCatalogService methodCatalog) => new()
        {
            Name = heat.Name,
            HeatNumber = heat.HeatNumber,
            GroupCount = heat.NumberOfGroups,
            ParticipantsPerGroup = heat.GroupCapacity,
            ScoringMode = heat.ScoringMode,
            Scoring = new ScoringConfig
            {
                Method = methodCatalog.CreateScoreMethod(heat.ScoreMethodId),
                UsePenalties = heat.UsePenalty,
            },
            Assignment = new AssignmentConfig
            {
                KartMethod = methodCatalog.CreateKartMethod(heat.KartMethodId),
                GroupMethod = methodCatalog.CreateGroupMethod(heat.GroupMethodId),
                Shuffle = methodCatalog.CreateShuffleMethod(heat.ShuffleMethodId)
            }
        };
    }
}
