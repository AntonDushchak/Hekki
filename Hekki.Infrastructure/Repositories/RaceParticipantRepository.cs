using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class RaceParticipantRepository : IRaceParticipantRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RaceParticipantRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<PilotDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participants = await db.RaceParticipants
                .Include(x => x.Pilot)
                .ToListAsync(ct);

            return participants.Select(p => RaceParticipantMapper.ToDto(p)).ToList();
        }

        public async Task<PilotDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participant = await db.RaceParticipants
                .Include(x => x.Pilot)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (participant == null)
            {
                return null;
            }

            return RaceParticipantMapper.ToDto(participant);
        }

        public async Task<IReadOnlyList<PilotDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participants = await db.RaceParticipants
                .Where(x => x.RaceId == raceId)
                .Include(x => x.Pilot)
                .ToListAsync(ct);

            return participants.Select(p => RaceParticipantMapper.ToDto(p)).ToList();
        }

        public async Task<int> AddAsync(int raceId, PilotDto participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = RaceParticipantMapper.ToEntity(participant, raceId);
            db.RaceParticipants.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(PilotDto participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { participant.ParticipantId }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Race participant with ID {participant.ParticipantId} not found");

            RaceParticipantMapper.UpdateEntity(entity, participant);
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

            return await db.RaceParticipants.AnyAsync(p => p.Id == id, ct);
        }
    }
}
