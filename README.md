# EfCoreSamples

Console app (.NET 9) using dependency injection and EF Core (SQL Server) with `Author` and `Book` models (one-to-many).

## How to run

- Update `appsettings.json` `DefaultConnection` for your SQL Server:
  - LocalDB: `Server=(localdb)\\MSSQLLocalDB;Database=EfCoreSamplesDb;Trusted_Connection=True;TrustServerCertificate=True`
  - Local SQL Server: `Server=.;Database=EfCoreSamplesDb;Trusted_Connection=True;TrustServerCertificate=True`
  - Docker SQL Server: `Server=localhost,1433;User Id=sa;Password=Your_password123;TrustServerCertificate=True`
- Apply migrations at startup (already configured via `Database.Migrate()`), then run:
  - `dotnet run`

The app seeds one author (Jane Austen) and one book (Pride and Prejudice) on first run and prints `Title — Author`.

## Migrations (optional via CLI)

- `dotnet tool restore`
- `dotnet tool run dotnet-ef migrations add AnotherChange`
- `dotnet tool run dotnet-ef database update`
