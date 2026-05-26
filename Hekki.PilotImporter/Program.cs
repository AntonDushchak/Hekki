using Hekki.Infrastructure;
using Hekki.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Hekki.PilotImporter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hekki Pilot Importer");
            Console.WriteLine("====================");
            Console.WriteLine();

            var optionsBuilder = new DbContextOptionsBuilder<HekkiDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=hekki;Username=postgres;Password=gmina531";
            optionsBuilder.UseNpgsql(connectionString);

            using var context = new HekkiDbContext(optionsBuilder.Options);

            await context.Database.EnsureCreatedAsync();

            var importService = new PilotImportService(context);

            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Hekki.Infrastructure", "jsconfig1.json");
            jsonFilePath = Path.GetFullPath(jsonFilePath);

            Console.WriteLine($"Importing pilots from: {jsonFilePath}");
            Console.WriteLine();

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"ERROR: File not found: {jsonFilePath}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                var pilotsBefore = await importService.GetPilotsCountAsync();
                Console.WriteLine($"Pilots in database before import: {pilotsBefore}");

                var importedCount = await importService.ImportPilotsFromJsonAsync(jsonFilePath);

                var pilotsAfter = await importService.GetPilotsCountAsync();
                Console.WriteLine($"Imported {importedCount} new pilots");
                Console.WriteLine($"Total pilots in database: {pilotsAfter}");
                Console.WriteLine();
                Console.WriteLine("Import completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
