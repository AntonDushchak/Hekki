using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class HeatEntryRepository : IHeatEntryRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public HeatEntryRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<HeatEntryDto>> GetByHeatIdAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .Where(x => x.HeatId == heatId)
                .Include(x => x.Participant)
                    .ThenInclude(x => x.Pilot)
                .ToListAsync(ct);

            return entries.Select(e => _mapper.Map<HeatEntryDto>(e)).ToList();
        }

        public async Task<IReadOnlyList<HeatEntryDto>> GetByHeatIdAndGroupAsync(int heatId, int groupNumber, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entries = await db.HeatEntries
                .Where(x => x.HeatId == heatId && x.GroupNumber == groupNumber)
                .Include(x => x.Participant)
                    .ThenInclude(x => x.Pilot)
                .ToListAsync(ct);

            return entries.Select(e => _mapper.Map<HeatEntryDto>(e)).ToList();
        }

        public async Task AddAsync(int heatId, int groupNumber, HeatEntryDto entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatEntryEntity>(entry);
            entity.HeatId = heatId;
            entity.GroupNumber = groupNumber;
            entity.SeedOrder = 0;
            db.HeatEntries.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int heatId, int groupNumber, HeatEntryDto entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatEntries
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == entry.ParticipantId, ct);

            if (entity is null)
                throw new InvalidOperationException($"Heat entry not found for HeatId {heatId} and ParticipantId {entry.ParticipantId}");

            entity.KartNumber = entry.KartNumber;
            entity.GridPosition = entry.GridPosition;
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
