# Импорт пилотов из CSV

## Описание

Сервис для импорта данных пилотов из CSV файла в базу данных.

## Структура

- **PilotImportService** (`Hekki.Infrastructure/Services/PilotImportService.cs`) - сервис импорта
- **Hekki.PilotImporter** - консольное приложение для запуска импорта

## Использование сервиса в коде

```csharp
using Hekki.Infrastructure;
using Hekki.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

// Настройка контекста
var optionsBuilder = new DbContextOptionsBuilder<HekkiDbContext>();
optionsBuilder.UseSqlServer("your_connection_string");

using var context = new HekkiDbContext(optionsBuilder.Options);
var importService = new PilotImportService(context);

// Импорт пилотов
var importedCount = await importService.ImportPilotsFromCsvAsync("path/to/pilots.csv");
Console.WriteLine($"Imported {importedCount} pilots");
```

## Запуск консольного импортера

1. Откройте `Hekki.PilotImporter/Program.cs`
2. Укажите строку подключения к вашей базе данных:
   ```csharp
   var connectionString = "Server=(localdb)\\mssqllocaldb;Database=HekkiDb;Trusted_Connection=True;";
   ```
3. Запустите проект `Hekki.PilotImporter`

## Особенности

- Импортируются поля: **First Name**, **Last Name**, **Profile URL**, **Photo URL**, **Team/Track**
- Поле **Country** читается как часть формата, но не сохраняется, так как в модели пилота нет отдельного поля страны
- Проверка на дубликаты: пилот не добавляется, если уже существует запись с такими же FirstName, LastName и ProfileUrl
- CSV-файл по умолчанию должен находиться по пути: `Hekki.Infrastructure/pilots.csv`

## Формат CSV

```csv
Driver Name,Profile URL,Photo URL,Team/Track,Country,First Name,Last Name
Pilot Name,https://example.com/profile,https://example.com/photo.jpg,Team Name,Country,Pilot,Name
```

## Альтернатива: Использование в UI

Вы можете интегрировать `PilotImportService` в ваше WPF приложение:

```csharp
public class ImportViewModel
{
	private readonly HekkiDbContext _context;

	public ImportViewModel(HekkiDbContext context)
	{
		_context = context;
	}

	public async Task ImportPilotsAsync(string jsonFilePath)
	{
		var importService = new PilotImportService(_context);
		var count = await importService.ImportPilotsFromCsvAsync(csvFilePath);
		MessageBox.Show($"Imported {count} pilots");
	}
}
```
