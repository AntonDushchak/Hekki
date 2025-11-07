## Hekki

Hekki is a WinForms desktop application for kart racing management, covering everything from heat generation to Excel-based reporting.

- **Hekki (UI)** — Windows Forms front end (`Hekki/Hekki.UI`) with dedicated forms for each regulation. The entry point (`Program.cs`) uses [Squirrel](https://github.com/clowd/Clowd.Squirrel) for automatic updates and launches the main window.
- **RaceLogic** — domain library that models pilots, regulations, and heat assignment algorithms. It also handles combination calculation and synchronization with spreadsheets.
- **ExcelController** — integration layer with Microsoft Excel via **Microsoft.Office.Interop.Excel**. Provides read/write access to race sheets, range cleanup, and test data import/export.
- **Tests** — `RegulationTests` and `ExcelWorkerTest` (.NET 6, **NUnit**). Cover heat assignment scenarios and Excel interop operations.

Typical workflow: operators manage heats through the WinForms UI; the UI calls `RaceLogic` services that read and write data through `ExcelWorker`. Automatic updates and distribution are powered by Squirrel (see `squirrelPublish`).
