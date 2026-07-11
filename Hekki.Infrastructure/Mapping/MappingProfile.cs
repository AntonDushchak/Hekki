using AutoMapper;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PilotEntity, PilotDto>().ReverseMap();

            // ===== Race =====
            CreateMap<RaceEntity, RaceDataDto>()
                .ForMember(d => d.RaceId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Participants, opt => opt.MapFrom(s => s.Participants))
                .ForMember(d => d.Heats, opt => opt.MapFrom(s => s.Heats));

            CreateMap<RaceDataDto, RaceEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RaceId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.RaceName))
                .ForMember(d => d.Regulation, opt => opt.Ignore())
                .ForMember(d => d.Participants, opt => opt.Ignore())
                .ForMember(d => d.Heats, opt => opt.Ignore());

            CreateMap<RaceEntity, RaceSummaryDto>()
                .ForMember(d => d.RaceId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Name));

            // ===== RaceParticipant =====
            CreateMap<RaceParticipantEntity, RaceParticipantDto>()
                .ForMember(d => d.ParticipantId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Pilot.Name))
                .ForMember(d => d.PhotoPath, opt => opt.MapFrom(s => s.Pilot.PhotoPath))
                .ForMember(d => d.League, opt => opt.MapFrom(s => s.League));

            CreateMap<RaceParticipantDto, RaceParticipantEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ParticipantId))
                .ForMember(d => d.Race, opt => opt.Ignore())
                .ForMember(d => d.Pilot, opt => opt.Ignore())
                .ForMember(d => d.HeatEntries, opt => opt.Ignore())
                .ForMember(d => d.HeatResults, opt => opt.Ignore());

            // ===== Heat =====
            CreateMap<HeatEntity, HeatDto>()
                .ForMember(d => d.HeatId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ScoringMode, opt => opt.MapFrom(s => (Application.Regulations.ScoringMode)s.ScoringMode))
                .ForMember(d => d.Groups, opt => opt.MapFrom(s => s.HeatGroups))
                .ForMember(d => d.GroupCount, opt => opt.MapFrom(s => s.HeatGroups.Count));

            CreateMap<HeatDto, HeatEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.HeatId))
                .ForMember(d => d.RaceId, opt => opt.Ignore())
                .ForMember(d => d.ScoringMode, opt => opt.MapFrom(s => (int)s.ScoringMode))
                .ForMember(d => d.Race, opt => opt.Ignore())
                .ForMember(d => d.Regulation, opt => opt.Ignore())
                .ForMember(d => d.HeatGroups, opt => opt.Ignore());

            // ===== HeatGroup =====
            CreateMap<HeatGroupEntity, HeatGroupDto>()
                .ForMember(d => d.Entries, opt => opt.MapFrom(s => s.Entries))
                .ForMember(d => d.Results, opt => opt.MapFrom(s => s.Results));

            CreateMap<HeatGroupDto, HeatGroupEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.HeatId, opt => opt.Ignore())
                .ForMember(d => d.Heat, opt => opt.Ignore())
                .ForMember(d => d.Entries, opt => opt.Ignore())
                .ForMember(d => d.Results, opt => opt.Ignore());

            // ===== HeatEntry =====
            CreateMap<HeatEntryEntity, HeatEntryDto>()
                .ForMember(d => d.PilotName, opt => opt.MapFrom(s => s.Participant.Pilot.Name))
                .ForMember(d => d.KartNumber, opt => opt.MapFrom(s => s.KartNumber))
                .ForMember(d => d.GridPosition, opt => opt.MapFrom(s => s.GridPosition));

            CreateMap<HeatEntryDto, HeatEntryEntity>()
                .ForMember(d => d.GroupId, opt => opt.Ignore())
                .ForMember(d => d.Group, opt => opt.Ignore())
                .ForMember(d => d.Participant, opt => opt.Ignore())
                .ForMember(d => d.SeedOrder, opt => opt.Ignore());

            // ===== HeatResult =====
            CreateMap<HeatResultEntity, HeatResultDto>();

            CreateMap<HeatResultDto, HeatResultEntity>()
                .ForMember(d => d.GroupId, opt => opt.Ignore())
                .ForMember(d => d.Group, opt => opt.Ignore())
                .ForMember(d => d.Participant, opt => opt.Ignore());

            // ===== Regulation =====
            CreateMap<RegulationEntity, RegulationSummaryDto>();
            CreateMap<RegulationSummaryDto, RegulationEntity>()
                .ForMember(d => d.Json, opt => opt.Ignore());

            CreateMap<RegulationEntity, RegulationEditDto>()
                .ForMember(d => d.Config, opt => opt.MapFrom(s =>
                    System.Text.Json.JsonSerializer.Deserialize<Application.Regulations.RegulationConfig>(s.Json)));

            CreateMap<RegulationEditDto, RegulationEntity>()
                .ForMember(d => d.Json, opt => opt.MapFrom(s =>
                    System.Text.Json.JsonSerializer.Serialize(s.Config)))
                .ForMember(d => d.CreationDate, opt => opt.Ignore())
                .ForMember(d => d.Version, opt => opt.Ignore());
        }
    }
}
