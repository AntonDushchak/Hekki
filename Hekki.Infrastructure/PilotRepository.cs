using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class PilotRepository : IPilotRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public PilotRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Pilot>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Pilots
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<Pilot?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            return entity?.ToDomain();
        }

        public async Task<int> AddAsync(Pilot pilot, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = pilot.ToEntity();
            db.Pilots.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(Pilot pilot, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots.FindAsync(new object[] { pilot.Id }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Pilot with ID {pilot.Id} not found");

            pilot.UpdateEntity(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.Pilots.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Pilots.AnyAsync(p => p.Id == id, ct);
        }
    }
}
