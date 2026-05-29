using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RaceRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<RaceDataDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var raceEntities = await db.Races
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatEntries)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatParticipantResults)
                .ToListAsync(ct);

            return raceEntities.Select(e => RaceMapper.ToDto(e)).ToList();
        }

        public async Task<RaceDataDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var raceEntity = await db.Races
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Pilot)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatEntries)
                .Include(x => x.Heats)
                    .ThenInclude(x => x.HeatParticipantResults)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (raceEntity == null)
            {
                return null; //TODO: error handler/logger
            }

            return RaceMapper.ToDto(raceEntity);
        }

        public async Task<int> AddAsync(RaceDataDto race, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = RaceMapper.ToEntity(race);
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

            RaceMapper.UpdateEntity(entity, race);
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
