using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class HeatEntryRepository : IHeatEntryRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public HeatEntryRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .Where(x => x.HeatId == heatId)
                .ToListAsync(ct);

            return entries.Select(e => HeatEntryMapper.ToDto(e)).ToList();
        }

        public async Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAndGroupAsync(int heatId, int groupNumber, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .Where(x => x.HeatId == heatId && x.GroupNumber == groupNumber)
                .ToListAsync(ct);

            return entries.Select(e => HeatEntryMapper.ToDto(e)).ToList();
        }

        public async Task AddAsync(int heatId, int groupNumber, HeatResultDto entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = HeatEntryMapper.ToEntity(entry, heatId, groupNumber);
            db.HeatEntries.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int heatId, int groupNumber, HeatResultDto entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatEntries
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == entry.ParticipantId, ct);

            if (entity is null)
                throw new InvalidOperationException($"Heat entry not found for HeatId {heatId} and ParticipantId {entry.ParticipantId}");

            HeatEntryMapper.UpdateEntity(entity, entry);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatEntries
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == participantId, ct);

            if (entity is null)
                return;

            db.HeatEntries.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.HeatEntries.AnyAsync(e => e.HeatId == heatId && e.ParticipantId == participantId, ct);
        }
    }
}
