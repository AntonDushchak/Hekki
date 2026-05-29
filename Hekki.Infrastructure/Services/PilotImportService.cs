using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hekki.Infrastructure.Services
{
    public class PilotImportService
    {
        private readonly HekkiDbContext _context;

        public PilotImportService(HekkiDbContext context)
        {
            _context = context;
        }

        public async Task<int> ImportPilotsFromJsonAsync(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var pilots = JsonSerializer.Deserialize<List<PilotJsonDto>>(jsonContent);

            if (pilots == null || pilots.Count == 0)
            {
                return 0;
            }

            int importedCount = 0;

            foreach (var pilotDto in pilots)
            {
                var existingPilot = await _context.Pilots
                    .FirstOrDefaultAsync(p => p.Name == pilotDto.DriverName && p.ProfileUrl == pilotDto.ProfileUrl);

                if (existingPilot == null)
                {
                    var pilotEntity = new PilotEntity
                    {
                        Name = pilotDto.DriverName,
                        ProfileUrl = pilotDto.ProfileUrl,
                        PhotoPath = pilotDto.PhotoUrl
                    };

                    _context.Pilots.Add(pilotEntity);
                    importedCount++;
                }
            }

            if (importedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return importedCount;
        }

        public async Task<int> GetPilotsCountAsync()
        {
            return await _context.Pilots.CountAsync();
        }
    }

    internal class PilotJsonDto
    {
        public string DriverName { get; set; }
        public string ProfileUrl { get; set; }
        public string PhotoUrl { get; set; }
    }
}
