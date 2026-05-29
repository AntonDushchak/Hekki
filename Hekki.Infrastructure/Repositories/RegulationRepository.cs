using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;
using Hekki.Application.Regulations;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hekki.Infrastructure.Repositories
{
    public class RegulationRepository : IRegulationRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RegulationRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<RegulationSummaryDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => ToDto(r))
                .ToListAsync(ct);
        }

        public async Task<RegulationSummaryDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations
                .AsNoTracking()
                .Select(r => ToDto(r))
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<RegulationEditDto> GetForEditAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(RegulationEditDto regulationDto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = new RegulationEntity
            {
                Name = regulationDto.Name,
                Json = JsonSerializer.Serialize(regulationDto.Config),
            };

            db.Regulations.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Regulations.FindAsync(new object[] { id }, ct);
            if (entity is null)
                return;

            db.Regulations.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations.AnyAsync(r => r.Id == id, ct);
        }

        private static RegulationEditDto ToEditDto(RegulationEntity entity)
        {
            return new RegulationEditDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Config = JsonSerializer.Deserialize<RegulationConfig>(entity.Json)
            };
        }

        private static RegulationSummaryDto ToDto(RegulationEntity entity)
        {
            return new RegulationSummaryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Version = entity.Version,
                CreationDate = entity.CreationDate
            };
        }

        private static RegulationEntity ToEntity(RegulationSummaryDto dto)
        {
            return new RegulationEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Version = dto.Version,
                CreationDate = dto.CreationDate
            };
        }
    }
}