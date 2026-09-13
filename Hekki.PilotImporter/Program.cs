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

            var connectionString = "";
            optionsBuilder.UseNpgsql(connectionString);

            using var context = new HekkiDbContext(optionsBuilder.Options);

            await context.Database.MigrateAsync();

            var importService = new PilotImportService(context);

            var csvFilePath = args.Length > 0
                ? Path.GetFullPath(args[0])
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Hekki.Infrastructure", "pilots.csv");
            csvFilePath = Path.GetFullPath(csvFilePath);

            Console.WriteLine($"Importing pilots from: {csvFilePath}");
            Console.WriteLine();

            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"ERROR: File not found: {csvFilePath}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                var pilotsBefore = await importService.GetPilotsCountAsync();
                Console.WriteLine($"Pilots in database before import: {pilotsBefore}");

                var importedCount = await importService.ImportPilotsFromCsvAsync(csvFilePath);

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
