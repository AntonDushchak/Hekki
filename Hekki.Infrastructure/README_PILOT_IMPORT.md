# Импорт пилотов из JSON

## Описание

Сервис для импорта данных пилотов из JSON файла в базу данных.

## Структура

- **PilotJsonDto** (`Hekki.Infrastructure/DTOs/PilotJsonDto.cs`) - DTO для десериализации JSON
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
var importedCount = await importService.ImportPilotsFromJsonAsync("path/to/jsconfig1.json");
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

- Импортируются только поля: **Name**, **ProfileUrl**, **PhotoPath**
- Поля **Team/Track** и **Country** игнорируются
- Проверка на дубликаты: пилот не добавляется, если уже существует запись с таким же Name и ProfileUrl
- JSON файл должен находиться по пути: `Hekki.Infrastructure/jsconfig1.json`

## Формат JSON

```json
[
  {
	"Driver Name": "Pilot Name",
	"Profile URL": "https://example.com/profile",
	"Photo URL": "https://example.com/photo.jpg",
	"Team/Track": "Team Name",
	"Country": "Country"
  }
]
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
		var count = await importService.ImportPilotsFromJsonAsync(jsonFilePath);
		MessageBox.Show($"Imported {count} pilots");
	}
}
```
