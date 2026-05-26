using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class RaceParticipantRepository : IRaceParticipantRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RaceParticipantRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<RaceParticipant>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.RaceParticipants
                .AsNoTracking()
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<RaceParticipant?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.Id == id, ct);

            return entity?.ToDomain();
        }

        public async Task<IReadOnlyList<RaceParticipant>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.RaceParticipants
                .AsNoTracking()
                .Where(rp => rp.RaceId == raceId)
                .OrderBy(rp => rp.Id)
                .ToListAsync(ct);

            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<int> AddAsync(RaceParticipant participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = participant.ToEntity();
            db.RaceParticipants.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(RaceParticipant participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { participant.Id }, ct);
            if (entity is null)
                throw new InvalidOperationException($"RaceParticipant with ID {participant.Id} not found");

            participant.UpdateEntity(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.RaceParticipants.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.RaceParticipants.AnyAsync(rp => rp.Id == id, ct);
        }

        public async Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.RaceParticipants
                .AnyAsync(rp => rp.RaceId == raceId && rp.PilotId == pilotId, ct);
        }
    }
}
