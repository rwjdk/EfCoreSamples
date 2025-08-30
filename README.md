# EfCoreSamples

.NET 9 console app using DI + EF Core (SQL Server) with `Author` and `Book` (one-to-many). Uses GUID IDs, data annotations, concrete repositories (no interfaces), and an arrow-key console UI.

## Features

- Models: `Author`, `Book` (GUID keys). `Book` has required `Title` and `Description`.
- Data access: `SqlDbContext` with repositories (`BookRepository`, `AuthorRepository`).
- Interactive console UI:
  - Main menu: List books, List authors, Add book, Remove book, Exit.
  - Keys: Up/Down to navigate, Enter to select, Esc/Backspace to go back, PageUp/PageDown to jump.
  - List books: select a book to view details.
  - List authors: select an author, then select among that author's books to view details.
  - Add book: prompts for Title, Author name, Description; press Esc at any prompt to cancel.
  - Remove book: select a book and confirm deletion.
- Startup applies migrations automatically (`Database.MigrateAsync`).
- First-run seeding: if DB is empty, inserts 10 well-known books from 7 authors with descriptions.

## Configure connection string

The app requires `ConnectionStrings:DefaultConnection`. Provide it via any of:

- `appsettings.json` (copied to output automatically; default uses localhost):
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=EfCoreSamplesDb;Trusted_Connection=True;TrustServerCertificate=True"
    }
  }
- Environment variable (PowerShell):
  `$env:ConnectionStrings__DefaultConnection = "Server=...;Database=...;..."`
- Command line:
  `dotnet run -- --ConnectionStrings:DefaultConnection "Server=...;Database=...;..."`

Notes:
- There is no fallback; the app throws if the connection string is missing.
- Use a server you can connect to and with permission to create/update the database (or pre-create the DB and grant access).
  - SQL Express named instance: `Server=localhost\\SQLEXPRESS;Trusted_Connection=True;TrustServerCertificate=True`
  - Docker SQL Server: `Server=localhost,1433;User Id=sa;Password=Your_password123;TrustServerCertificate=True`

## Migrations

- Create migrations yourself (PMC or CLI). The app applies them at startup.
- Examples:
  - PMC: `Add-Migration Initial` then `Update-Database` (optional because app migrates on start)
  - CLI: `dotnet tool restore` then `dotnet tool run dotnet-ef migrations add Initial`

## Run

- `dotnet run`

## Project layout

- `SqlDbContext.cs`
- `Models/Author.cs`, `Models/Book.cs`
- `Repositories/BookRepository.cs`, `Repositories/AuthorRepository.cs`
- `Services/DatabaseSeeder.cs`, `Services/ConsoleUi.cs`
- `Program.cs`

## Educational notes

- Program.cs is annotated at EF-related lines: connection string, AddDbContext, scoped DbContext, MigrateAsync.
- Repositories and DatabaseSeeder include comments on Include, tracking vs AsNoTracking, find-or-create patterns, FK vs navigation, and SaveChanges batching.
