using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class HeatRepository : IHeatRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public HeatRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<HeatDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntities = await db.Heats
                .Include(x => x.HeatEntries)
                .Include(x => x.HeatParticipantResults)
                .ToListAsync(ct);

            return heatEntities.Select(e => HeatMapper.ToDto(e)).ToList();
        }

        public async Task<HeatDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntity = await db.Heats
                .Include(x => x.HeatEntries)
                .Include(x => x.HeatParticipantResults)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (heatEntity == null)
            {
                return null;
            }

            return HeatMapper.ToDto(heatEntity);
        }

        public async Task<IReadOnlyList<HeatDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntities = await db.Heats
                .Where(x => x.RaceId == raceId)
                .Include(x => x.HeatEntries)
                .Include(x => x.HeatParticipantResults)
                .ToListAsync(ct);

            return heatEntities.Select(e => HeatMapper.ToDto(e)).ToList();
        }

        public async Task<int> AddAsync(HeatDto heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = HeatMapper.ToEntity(heat);
            db.Heats.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(HeatDto heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Heats.FindAsync(new object[] { heat.HeatId }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Heat with ID {heat.HeatId} not found");

            HeatMapper.UpdateEntity(entity, heat);
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
