using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hekki.Infrastructure
{
    public class RegulationRepository : IRegulationRepository
    {
        static readonly JsonSerializerOptions JsonOpts = new()
        {
            
        };
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RegulationRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Regulation>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var rawData = await db.Regulations
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(e => new { e.Id, e.Name, e.Version, e.Json })
                .ToListAsync(ct);

            return rawData.Select(e => new Regulation
            {
                Id = e.Id,
                Name = e.Name,
                Version = e.Version,
                Configurations = DeserializeConfigurations(e.Json)
            }).ToList();
        }

        public async Task<IReadOnlyList<Regulation>> GetLookupAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(e => new Regulation() { Id = e.Id, Name = e.Name, Version = e.Version })
                .ToListAsync(ct);
        }

        public async Task<Regulation?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var e = await db.Regulations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return null;

            var configs = DeserializeConfigurations(e.Json);

            return new Regulation
            {
                Id = e.Id,
                Name = e.Name,
                Version = e.Version,
                Configurations = configs
            };
        }

        private static string SerializeConfigurations(List<HeatConfigurationModel> configs)
            => JsonSerializer.Serialize(configs ?? [], JsonOpts);

        private static List<HeatConfigurationModel> DeserializeConfigurations(string? json)
            => JsonSerializer.Deserialize<List<HeatConfigurationModel>>(json ?? "[]", JsonOpts) ?? [];

        public Task UpdateAsync(Regulation regulation, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(Regulation regulation, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.Regulations.FindAsync(regulation.Id, ct)
                         ?? new RegulationEntity { Id = regulation.Id };

            entity.Name = regulation.Name;
            entity.Version = regulation.Version;

            entity.Json = SerializeConfigurations(regulation.Configurations);

            db.Update(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public Task DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
