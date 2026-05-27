using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class HeatEntryRepository : IHeatEntryRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public HeatEntryRepository(IDbContextFactory<HekkiDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<IReadOnlyList<HeatEntry>> GetByHeatIdAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .AsNoTracking()
                .Where(he => he.HeatId == heatId)
                .OrderBy(he => he.ParticipantId)
                .ToListAsync(ct);

            return entries.Select(MapToModel).ToList();
        }

        public async Task<IReadOnlyList<HeatEntry>> GetByHeatIdsAsync(IEnumerable<int> heatIds, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .AsNoTracking()
                .Where(he => heatIds.Contains(he.HeatId))
                .OrderBy(he => he.HeatId)
                .ThenBy(he => he.ParticipantId)
                .ToListAsync(ct);

            return entries.Select(MapToModel).ToList();
        }

        public async Task AddAsync(HeatEntry entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = MapToEntity(entry);
            db.HeatEntries.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(HeatEntry entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatEntries
                .FirstOrDefaultAsync(he => he.HeatId == entry.HeatId && he.ParticipantId == entry.ParticipantId, ct);

            if (entity == null)
                throw new InvalidOperationException($"Heat entry not found: HeatId={entry.HeatId}, ParticipantId={entry.ParticipantId}");

            entity.KartNumber = entry.KartNumber;
            entity.GroupNumber = entry.GroupNumber;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatEntries
                .FirstOrDefaultAsync(he => he.HeatId == heatId && he.ParticipantId == participantId, ct);

            if (entity != null)
            {
                db.HeatEntries.Remove(entity);
                await db.SaveChangesAsync(ct);
            }
        }

        private static HeatEntry MapToModel(Entities.HeatEntryEntity entity)
        {
            return new HeatEntry
            {
                HeatId = entity.HeatId,
                ParticipantId = entity.ParticipantId,
                KartNumber = entity.KartNumber,
                GroupNumber = entity.GroupNumber
            };
        }

        private static Entities.HeatEntryEntity MapToEntity(HeatEntry model)
        {
            return new Entities.HeatEntryEntity
            {
                HeatId = model.HeatId,
                ParticipantId = model.ParticipantId,
                KartNumber = model.KartNumber,
                GroupNumber = model.GroupNumber
            };
        }
    }
}
