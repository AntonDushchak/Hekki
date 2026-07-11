using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public RaceRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<RaceDataDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var raceEntities = await db.Races
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatGroups)
                        .ThenInclude(x => x.Entries)
                            .ThenInclude(x => x.Participant)
                                .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatGroups)
                        .ThenInclude(x => x.Results)
                .ToListAsync(ct);

            return raceEntities.Select(e => _mapper.Map<RaceDataDto>(e)).ToList();
        }

        public async Task<RaceDataDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var raceEntity = await db.Races
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatGroups)
                        .ThenInclude(x => x.Entries)
                            .ThenInclude(x => x.Participant)
                                .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatGroups)
                        .ThenInclude(x => x.Results)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (raceEntity == null)
            {
                return null; //TODO: error handler/logger
            }

            return _mapper.Map<RaceDataDto>(raceEntity);
        }

        public async Task<int> AddAsync(RaceDataDto race, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<RaceEntity>(race);

            db.Races.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(RaceDataDto race, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Races.FindAsync(new object[] { race.RaceId }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Race with ID {race.RaceId} not found");

            var utcDate = race.Date.Kind == DateTimeKind.Utc ? race.Date : DateTime.SpecifyKind(race.Date, DateTimeKind.Utc);

            entity.Name = race.RaceName;
            entity.Date = utcDate;
            entity.Location = race.Location;
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Races.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.Races.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Races.AnyAsync(r => r.Id == id, ct);
        }
    }
}
