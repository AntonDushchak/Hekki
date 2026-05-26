using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class RaceRepository : IRaceRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RaceRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Race>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Races
                .AsNoTracking()
                .OrderByDescending(r => r.Date)
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<Race?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Races
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            return entity?.ToDomain();
        }

        public async Task<int> AddAsync(Race race, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = race.ToEntity();
            db.Races.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(Race race, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Races.FindAsync(new object[] { race.Id }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Race with ID {race.Id} not found");

            race.UpdateEntity(entity);
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
