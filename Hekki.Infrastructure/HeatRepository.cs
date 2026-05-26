using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class HeatRepository : IHeatRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public HeatRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Heat>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Heats
                .AsNoTracking()
                .ToListAsync(ct);

            return entities.Select(e => e.ToHeatDomain()).ToList();
        }

        public async Task<Heat?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Heats
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id, ct);

            return entity?.ToHeatDomain();
        }

        public async Task<IReadOnlyList<Heat>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Heats
                .AsNoTracking()
                .Where(h => h.RaceId == raceId)
                .OrderBy(h => h.ConfigurationIndex)
                .ToListAsync(ct);

            return entities.Select(e => e.ToHeatDomain()).ToList();
        }

        public async Task<int> AddAsync(Heat heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = heat.ToHeatEntity();
            db.Heats.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(Heat heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Heats.FindAsync(new object[] { heat.Id }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Heat with ID {heat.Id} not found");

            heat.UpdateHeatEntity(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Heats.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.Heats.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Heats.AnyAsync(h => h.Id == id, ct);
        }
    }
}
