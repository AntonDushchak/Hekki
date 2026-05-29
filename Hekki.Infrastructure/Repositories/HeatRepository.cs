using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class HeatRepository : IHeatRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public HeatRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<HeatDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntities = await db.Heats
                .Include(x => x.HeatEntries)
                    .ThenInclude(e => e.Participant)
                        .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatParticipantResults)
                .ToListAsync(ct);

            return heatEntities.Select(e => _mapper.Map<HeatDto>(e)).ToList();
        }

        public async Task<HeatDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntity = await db.Heats
                .Include(x => x.HeatEntries)
                    .ThenInclude(e => e.Participant)
                        .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatParticipantResults)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (heatEntity == null)
            {
                return null;
            }

            return _mapper.Map<HeatDto>(heatEntity);
        }

        public async Task<IReadOnlyList<HeatDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntities = await db.Heats
                .Where(x => x.RaceId == raceId)
                .Include(x => x.HeatEntries)
                    .ThenInclude(e => e.Participant)
                        .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatParticipantResults)
                .ToListAsync(ct);

            return heatEntities.Select(e => _mapper.Map<HeatDto>(e)).ToList();
        }

        public async Task<int> AddAsync(HeatDto heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatEntity>(heat);
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

            entity.Name = heat.Name;
            entity.HeatNumber = heat.HeatNumber;
            entity.ConfigurationIndex = heat.ConfigurationIndex;
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
