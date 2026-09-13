using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure.Services
{
    public class PilotImportService
    {
        private readonly HekkiDbContext _context;

        public PilotImportService(HekkiDbContext context)
        {
            _context = context;
        }

        public async Task<int> ImportPilotsFromCsvAsync(string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                throw new FileNotFoundException($"CSV file not found: {csvFilePath}");
            }

            var csvContent = await File.ReadAllTextAsync(csvFilePath);
            var rows = ParseCsv(csvContent);

            if (rows.Count < 2)
            {
                return 0;
            }

            var headers = rows[0]
                .Select((header, index) => new { Header = header.Trim(), Index = index })
                .ToDictionary(x => x.Header, x => x.Index, StringComparer.OrdinalIgnoreCase);

            var requiredHeaders = new[] { "First Name", "Last Name", "Profile URL", "Photo URL", "Team/Track" };
            var missingHeaders = requiredHeaders.Where(header => !headers.ContainsKey(header)).ToArray();
            if (missingHeaders.Length > 0)
            {
                throw new FormatException($"CSV is missing required columns: {string.Join(", ", missingHeaders)}");
            }

            int importedCount = 0;

            foreach (var row in rows.Skip(1))
            {
                var firstName = GetValue(row, headers["First Name"]);
                var lastName = GetValue(row, headers["Last Name"]);
                var profileUrl = GetValue(row, headers["Profile URL"]);

                if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                {
                    continue;
                }

                var existingPilot = await _context.Pilots
                    .FirstOrDefaultAsync(p => p.FirstName == firstName && p.LastName == lastName && p.ProfileUrl == profileUrl);

                if (existingPilot == null)
                {
                    var pilotEntity = new PilotEntity
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        ProfileUrl = profileUrl,
                        PhotoPath = GetValue(row, headers["Photo URL"]),
                        Team = GetValue(row, headers["Team/Track"])
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

        private static string GetValue(IReadOnlyList<string> row, int index)
        {
            return index < row.Count ? row[index].Trim() : string.Empty;
        }

        private static List<List<string>> ParseCsv(string content)
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var value = new System.Text.StringBuilder();
            var insideQuotes = false;

            for (var index = 0; index < content.Length; index++)
            {
                var character = content[index];
                if (character == '"')
                {
                    if (insideQuotes && index + 1 < content.Length && content[index + 1] == '"')
                    {
                        value.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (character == ',' && !insideQuotes)
                {
                    row.Add(value.ToString());
                    value.Clear();
                }
                else if ((character == '\r' || character == '\n') && !insideQuotes)
                {
                    if (character == '\r' && index + 1 < content.Length && content[index + 1] == '\n')
                    {
                        index++;
                    }

                    row.Add(value.ToString());
                    value.Clear();
                    if (row.Any(field => !string.IsNullOrWhiteSpace(field)))
                    {
                        rows.Add(row);
                    }
                    row = [];
                }
                else
                {
                    value.Append(character);
                }
            }

            row.Add(value.ToString());
            if (row.Any(field => !string.IsNullOrWhiteSpace(field)))
            {
                rows.Add(row);
            }

            return rows;
        }
    }
}
