using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class RaceParticipantRepository : IRaceParticipantRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public RaceParticipantRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<RaceParticipantDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participants = await db.RaceParticipants
                .Include(x => x.Pilot)
                .ToListAsync(ct);

            return participants.Select(p => _mapper.Map<RaceParticipantDto>(p)).ToList();
        }

        public async Task<RaceParticipantDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participant = await db.RaceParticipants
                .Include(x => x.Pilot)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (participant == null)
            {
                return null;
            }

            return _mapper.Map<RaceParticipantDto>(participant);
        }

        public async Task<IReadOnlyList<RaceParticipantDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participants = await db.RaceParticipants
                .Where(x => x.RaceId == raceId)
                .Include(x => x.Pilot)
                .ToListAsync(ct);

            return participants.Select(p => _mapper.Map<RaceParticipantDto>(p)).ToList();
        }

        public async Task<int> AddAsync(int raceId, RaceParticipantDto participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<RaceParticipantEntity>(participant);
            entity.RaceId = raceId;
            entity.IsActive = true;
            db.RaceParticipants.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(RaceParticipantDto participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { participant.ParticipantId }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Race participant with ID {participant.ParticipantId} not found");

            entity.Team = participant.Team;
            entity.League = participant.League;
            entity.IsActive = participant.IsActive;
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
