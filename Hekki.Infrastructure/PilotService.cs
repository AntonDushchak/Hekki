using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class PilotService : IPilotService
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public PilotService(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Pilot>> GetAllPilotsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Pilots
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<Pilot?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == pilotId, ct);

            return entity?.ToDomain();
        }

        public async Task<IReadOnlyList<Pilot>> SearchPilotsByNameAsync(string searchText, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Pilots
                .AsNoTracking()
                .Where(p => p.Name.Contains(searchText))
                .OrderBy(p => p.Name)
                .Take(20)
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }
    }
}
