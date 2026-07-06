using AutoMapper;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;

namespace Hekki.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // PilotEntity <-> PilotDto
            CreateMap<PilotEntity, PilotDto>().ReverseMap();

            // RaceEntity <-> RaceDataDto
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

            CreateMap<RaceEntity, RaceSummartDto>()
                .ForMember(d => d.RaceId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Name));

            CreateMap<RaceSummartDto, RaceEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RaceId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.RaceName))
                .ForMember(d => d.Regulation, opt => opt.Ignore())
                .ForMember(d => d.Participants, opt => opt.Ignore())
                .ForMember(d => d.Heats, opt => opt.Ignore());

            // RaceParticipantEntity <-> RaceParticipantDto
            CreateMap<RaceParticipantEntity, RaceParticipantDto>()
                .ForMember(d => d.ParticipantId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Pilot.Name))
                .ForMember(d => d.PhotoPath, opt => opt.MapFrom(s => s.Pilot.PhotoPath));

            CreateMap<RaceParticipantDto, RaceParticipantEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ParticipantId))
                .ForMember(d => d.Race, opt => opt.Ignore())
                .ForMember(d => d.Pilot, opt => opt.Ignore())
                .ForMember(d => d.HeatEntries, opt => opt.Ignore())
                .ForMember(d => d.HeatResults, opt => opt.Ignore());

            // HeatEntity <-> HeatDto
            CreateMap<HeatEntity, HeatDto>()
                .ForMember(d => d.HeatId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Groups, opt => opt.MapFrom(s => 
                    s.HeatEntries
                        .GroupBy(e => e.GroupNumber)
                        .Select(g => new HeatGroupDto
                        {
                            HeatId = s.Id,
                            GroupNumber = g.Key,
                            GroupIndex = g.Key - 1,
                            GroupCapacity = g.Count(),
                            Entries = g.Select(e => new HeatEntryDto
                            {
                                ParticipantId = e.ParticipantId,
                                PilotName = e.Participant.Pilot.Name,
                                KartNumber = e.KartNumber ?? 0,
                                GridPosition = e.GridPosition ?? 0
                            }).ToList(),
                            Results = s.HeatParticipantResults
                                .Where(r => g.Any(e => e.ParticipantId == r.ParticipantId))
                                .Select(r => new HeatResultDto
                                {
                                    ParticipantId = r.ParticipantId,
                                    FinishPosition = r.FinishPosition,
                                    TotalTimeMs = r.TotalTimeMs,
                                    BestLapMs = r.BestLapMs,
                                    Laps = r.Laps
                                }).ToList()
                        }).ToList()))
                .ForMember(d => d.GroupCount, opt => opt.Ignore());

            CreateMap<HeatDto, HeatEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.HeatId))
                .ForMember(d => d.Race, opt => opt.Ignore())
                .ForMember(d => d.Regulation, opt => opt.Ignore())
                .ForMember(d => d.HeatEntries, opt => opt.Ignore())
                .ForMember(d => d.HeatParticipantResults, opt => opt.Ignore());

            // HeatEntryEntity <-> HeatEntryDto
            CreateMap<HeatEntryEntity, HeatEntryDto>()
                .ForMember(d => d.PilotName, opt => opt.MapFrom(s => s.Participant.Pilot.Name))
                .ForMember(d => d.KartNumber, opt => opt.MapFrom(s => s.KartNumber ?? 0))
                .ForMember(d => d.GridPosition, opt => opt.MapFrom(s => s.GridPosition ?? 0));

            CreateMap<HeatEntryDto, HeatEntryEntity>()
                .ForMember(d => d.Heat, opt => opt.Ignore())
                .ForMember(d => d.Participant, opt => opt.Ignore())
                .ForMember(d => d.HeatId, opt => opt.Ignore())
                .ForMember(d => d.GroupNumber, opt => opt.Ignore())
                .ForMember(d => d.SeedOrder, opt => opt.Ignore());

            // HeatResultEntity <-> HeatResultDto
            CreateMap<HeatResultEntity, HeatResultDto>()
                .ForMember(d => d.Score, opt => opt.Ignore())
                .ForMember(d => d.Penalty, opt => opt.Ignore());

            CreateMap<HeatResultDto, HeatResultEntity>()
                .ForMember(d => d.Heat, opt => opt.Ignore())
                .ForMember(d => d.Participant, opt => opt.Ignore())
                .ForMember(d => d.HeatId, opt => opt.Ignore());

            // RegulationEntity <-> RegulationSummaryDto
            CreateMap<RegulationEntity, RegulationSummaryDto>();

            CreateMap<RegulationSummaryDto, RegulationEntity>()
                .ForMember(d => d.Json, opt => opt.Ignore());

            // RegulationEntity <-> RegulationEditDto
            CreateMap<RegulationEntity, RegulationEditDto>()
                .ForMember(d => d.Config, opt => opt.MapFrom(s => 
                    System.Text.Json.JsonSerializer.Deserialize<Hekki.Application.Regulations.RegulationConfig>(s.Json)));

            CreateMap<RegulationEditDto, RegulationEntity>()
                .ForMember(d => d.Json, opt => opt.MapFrom(s => 
                    System.Text.Json.JsonSerializer.Serialize(s.Config)))
                .ForMember(d => d.CreationDate, opt => opt.Ignore())
                .ForMember(d => d.Version, opt => opt.Ignore());
        }
    }
}
