using AutoMapper;
using Hekki.Application.Abstractions;
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

        public async Task<RaceParticipantDto?> GetByIdAsync(Guid participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var participant = await db.RaceParticipants
                .Include(x => x.Pilot)
                .FirstOrDefaultAsync(x => x.Id == participantId, ct);

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

        public async Task<Guid> AddAsync(RaceParticipantDto participant, int raceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var maxSortOrder = await db.RaceParticipants
                .Where(x => x.RaceId == raceId)
                .MaxAsync(x => (int?)x.SortOrder, ct);

            var entity = _mapper.Map<RaceParticipantEntity>(participant);
            entity.RaceId = raceId;
            entity.IsActive = true;
            entity.SortOrder = (maxSortOrder ?? -1) + 1;
            db.RaceParticipants.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateOrderAsync(int raceId, IReadOnlyList<Guid> orderedIds, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            var participants = await db.RaceParticipants
                .Where(x => x.RaceId == raceId)
                .ToDictionaryAsync(x => x.Id, ct);

            if (orderedIds.Count != participants.Count || orderedIds.Any(id => !participants.ContainsKey(id)))
                throw new InvalidOperationException($"Participant order for race {raceId} does not match its participants.");

            for (var i = 0; i < orderedIds.Count; i++)
                participants[orderedIds[i]].SortOrder = -(i + 1);
            await db.SaveChangesAsync(ct);

            for (var i = 0; i < orderedIds.Count; i++)
                participants[orderedIds[i]].SortOrder = i;
            await db.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }

        public async Task UpdateAsync(RaceParticipantDto participant, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { participant.ParticipantId }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Race participant with ID {participant.ParticipantId} not found");

            entity.FirstName = participant.FirstName;
            entity.LastName = participant.LastName;
            entity.Team = participant.Team;
            entity.League = participant.League;
            entity.IsActive = participant.IsActive;
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.RaceParticipants.FindAsync(new object[] { participantId }, ct);
            if (entity is null)
                return;

            db.RaceParticipants.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid participantId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.RaceParticipants.AnyAsync(p => p.Id == participantId, ct);
        }
    }
}
