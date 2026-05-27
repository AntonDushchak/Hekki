using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class HeatResultRepository : IHeatResultRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public HeatResultRepository(IDbContextFactory<HekkiDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<IReadOnlyList<HeatResult>> GetByHeatIdAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var results = await db.HeatResults
                .AsNoTracking()
                .Where(hr => hr.HeatId == heatId)
                .OrderBy(hr => hr.FinishPosition)
                .ToListAsync(ct);

            return results.Select(MapToModel).ToList();
        }

        public async Task<IReadOnlyList<HeatResult>> GetByHeatIdsAsync(IEnumerable<int> heatIds, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var results = await db.HeatResults
                .AsNoTracking()
                .Where(hr => heatIds.Contains(hr.HeatId))
                .OrderBy(hr => hr.HeatId)
                .ThenBy(hr => hr.FinishPosition)
                .ToListAsync(ct);

            return results.Select(MapToModel).ToList();
        }

        public async Task<IReadOnlyList<HeatResultWithPilotInfo>> GetByHeatIdsWithPilotInfoAsync(IEnumerable<int> heatIds, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var results = await db.HeatResults
                .AsNoTracking()
                .Include(hr => hr.Participant)
                .ThenInclude(p => p.Pilot)
                .Where(hr => heatIds.Contains(hr.HeatId))
                .OrderBy(hr => hr.HeatId)
                .ThenBy(hr => hr.FinishPosition)
                .Select(r => new HeatResultWithPilotInfo
                {
                    HeatId = r.HeatId,
                    ParticipantId = r.ParticipantId,
                    FinishPosition = r.FinishPosition,
                    TotalTimeMs = r.TotalTimeMs,
                    BestLapMs = r.BestLapMs,
                    Laps = r.Laps,
                    Status = r.Status,
                    PilotName = r.Participant.Pilot.Name
                })
                .ToListAsync(ct);

            return results;
        }

        public async Task AddAsync(HeatResult result, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = MapToEntity(result);
            db.HeatResults.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(HeatResult result, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatResults
                .FirstOrDefaultAsync(hr => hr.HeatId == result.HeatId && hr.ParticipantId == result.ParticipantId, ct);

            if (entity == null)
                throw new InvalidOperationException($"Heat result not found: HeatId={result.HeatId}, ParticipantId={result.ParticipantId}");

            entity.FinishPosition = result.FinishPosition;
            entity.TotalTimeMs = result.TotalTimeMs;
            entity.BestLapMs = result.BestLapMs;
            entity.Laps = result.Laps;
            entity.Status = result.Status;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int heatId, int participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.HeatResults
                .FirstOrDefaultAsync(hr => hr.HeatId == heatId && hr.ParticipantId == participantId, ct);

            if (entity != null)
            {
                db.HeatResults.Remove(entity);
                await db.SaveChangesAsync(ct);
            }
        }

        private static HeatResult MapToModel(Entities.HeatResultEntity entity)
        {
            return new HeatResult
            {
                HeatId = entity.HeatId,
                ParticipantId = entity.ParticipantId,
                FinishPosition = entity.FinishPosition,
                TotalTimeMs = entity.TotalTimeMs,
                BestLapMs = entity.BestLapMs,
                Laps = entity.Laps,
                Status = entity.Status
            };
        }

        private static Entities.HeatResultEntity MapToEntity(HeatResult model)
        {
            return new Entities.HeatResultEntity
            {
                HeatId = model.HeatId,
                ParticipantId = model.ParticipantId,
                FinishPosition = model.FinishPosition,
                TotalTimeMs = model.TotalTimeMs,
                BestLapMs = model.BestLapMs,
                Laps = model.Laps,
                Status = model.Status
            };
        }
    }
}
