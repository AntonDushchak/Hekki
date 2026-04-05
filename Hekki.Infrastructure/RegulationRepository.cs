using Hekki.Domain.Models;
using Hekki.Application.Abstrations;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class RegulationRepository : IRegulationRepository
    {
        private readonly IDbContextFactory<HekkiDbContext> _dbFactory;

        public RegulationRepository(IDbContextFactory<HekkiDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<Regulation>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Regulations
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(e => new Regulation
                {
                    Id = e.Id,
                    Name = e.Name,
                    Json = e.Json
                })
                .ToListAsync(ct);
        }

        public async Task<Regulation?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var e = await db.Regulations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return null;

            return new Regulation { Id = e.Id, Name = e.Name, Json = e.Json };
        }

        public Task UpdateAsync(Regulation regulation, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddAsync(Regulation regulation, CancellationToken ct = default)
        {
            throw new NotImplementedException();
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
