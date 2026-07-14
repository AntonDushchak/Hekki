using AutoMapper;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Regulation;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Repositories
{
    public class RegulationRepository : IRegulationRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;
        private readonly IMapper _mapper;

        public RegulationRepository(IDbContextFactory<HekkiDbContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<RegulationSummaryDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entities = await db.Regulations
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync(ct);

            return entities.Select(e => _mapper.Map<RegulationSummaryDto>(e)).ToList();
        }

        public async Task<RegulationSummaryDto?> GetByIdAsync(int regulationId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Regulations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == regulationId, ct);

            return entity is null ? null : _mapper.Map<RegulationSummaryDto>(entity);
        }

        public async Task<RegulationEditDto?> GetForEditAsync(int regulationId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var regulation = await db.Regulations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == regulationId, ct);

            return regulation is null ? null : _mapper.Map<RegulationEditDto>(regulation);
        }

        public async Task<int> AddAsync(RegulationEditDto regulationDto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = _mapper.Map<RegulationEntity>(regulationDto);
            entity.CreationDate = DateTime.UtcNow;
            entity.Version = 1; //TODO: Implement versioning logic if needed

            db.Regulations.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task DeleteAsync(int regulationId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Regulations.FindAsync(new object[] { regulationId }, ct);
            if (entity is null)
                return;

            db.Regulations.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int regulationId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations.AnyAsync(r => r.Id == regulationId, ct);
        }
    }
}