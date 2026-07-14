using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
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
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Entries)
                        .ThenInclude(e => e.Participant)
                            .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Results)
                .ToListAsync(ct);

            return heatEntities.Select(e => _mapper.Map<HeatDto>(e)).ToList();
        }

        public async Task<HeatDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntity = await db.Heats
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Entries)
                        .ThenInclude(e => e.Participant)
                            .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Results)
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
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Entries)
                        .ThenInclude(e => e.Participant)
                            .ThenInclude(p => p.Pilot)
                .Include(x => x.HeatGroups)
                    .ThenInclude(g => g.Results)
                .ToListAsync(ct);

            return heatEntities.Select(e => _mapper.Map<HeatDto>(e)).ToList();
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

        public async Task DeleteAsync(int headId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Heats.FindAsync(new object[] { headId }, ct);
            if (entity is null)
                return;

            db.Heats.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int heatId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Heats.AnyAsync(h => h.Id == heatId, ct);
        }

        public async Task AddHeatEntryAsync(int heatId, int groupId, HeatEntryDto entry, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatEntryEntity>(entry);
            entity.GroupId = groupId;
            entity.SeedOrder = 0;
            db.HeatEntries.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task AddHeatResultAsync(int heatId, int groupId, HeatResultDto result, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatResultEntity>(result);
            entity.GroupId = groupId;
            db.HeatResults.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<int> AddHeatWithGroupsAsync(int raceId, HeatDto heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var heatEntity = _mapper.Map<HeatEntity>(heat);
            heatEntity.RaceId = raceId;

            db.Heats.Add(heatEntity);
            await db.SaveChangesAsync(ct);
            return heatEntity.Id;
        }

        public async Task AddGroupsAsync(int heatId, IReadOnlyList<HeatGroupDto> groups, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = _mapper.Map<List<HeatGroupEntity>>(groups);
            foreach (var e in entities) e.HeatId = heatId;

            db.HeatGroups.AddRange(entities);
            await db.SaveChangesAsync(ct);
        }

        public async Task<int> AddAsync(int raceId, HeatDto heat, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatEntity>(heat);
            entity.RaceId = raceId;
            db.Heats.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task<int> AddGroupAsync(int heatId, HeatGroupDto group, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<HeatGroupEntity>(group);
            entity.HeatId = heatId;
            db.HeatGroups.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }
    }
}
