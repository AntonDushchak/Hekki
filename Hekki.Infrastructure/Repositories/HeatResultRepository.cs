using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class HeatResultRepository : IHeatResultRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public HeatResultRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<HeatResultDto>> GetByHeatIdAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var results = await db.HeatResults
                .Where(x => x.HeatId == heatId)
                .ToListAsync(ct);

            return results.Select(r => _mapper.Map<HeatResultDto>(r)).ToList();
        }

        public async Task<HeatResultDto?> GetByHeatAndParticipantAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var result = await db.HeatResults
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == participantId, ct);

            if (result == null)
            {
                return null;
            }

            return _mapper.Map<HeatResultDto>(result);
        }

        public async Task AddAsync(int heatId, HeatResultDto result, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatResultEntity>(result);
            entity.HeatId = heatId;
            db.HeatResults.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int heatId, HeatResultDto result, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatResults
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == result.ParticipantId, ct);

            if (entity is null)
                throw new InvalidOperationException($"Heat result not found for HeatId {heatId} and ParticipantId {result.ParticipantId}");

            entity.FinishPosition = result.FinishPosition;
            entity.TotalTimeMs = result.TotalTimeMs;
            entity.BestLapMs = result.BestLapMs;
            entity.Laps = result.Laps;
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatResults
                .FirstOrDefaultAsync(x => x.HeatId == heatId && x.ParticipantId == participantId, ct);

            if (entity is null)
                return;

            db.HeatResults.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.HeatResults.AnyAsync(r => r.HeatId == heatId && r.ParticipantId == participantId, ct);
        }
    }
}
