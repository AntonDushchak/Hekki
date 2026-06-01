using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class PilotRepository : IPilotRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public PilotRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<PilotDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Pilots
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(ct);

            return entities.Select(e => _mapper.Map<PilotDto>(e)).ToList();
        }

        public async Task<PilotDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            return entity == null ? null : _mapper.Map<PilotDto>(entity);
        }

        public async Task<int> AddAsync(PilotDto pilot, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<PilotEntity>(pilot);
            db.Pilots.Add(entity);
            await db.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task UpdateAsync(PilotDto pilot, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots.FindAsync(new object[] { pilot.Id }, ct);
            if (entity is null)
                throw new InvalidOperationException($"Pilot with ID {pilot.Id} not found");

            _mapper.Map(pilot, entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Pilots.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.Pilots.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Pilots.AnyAsync(p => p.Id == id, ct);
        }

        public async Task<IReadOnlyList<PilotDto>> SearchByNameAsync(string searchText, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entities = await db.Pilots
                .AsNoTracking()
                .Where(p => p.Name.Contains(searchText))
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
            return entities.Select(e => _mapper.Map<PilotDto>(e)).ToList();
        }
    }
}
