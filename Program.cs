using EfCoreSamples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EfCoreSamples.Repositories;
using EfCoreSamples.Services;

var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings { Args = args });

// Config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(connectionString));

// App service
builder.Services.AddScoped<AppRunner>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

var host = builder.Build();

// Run a scoped operation
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
await db.Database.MigrateAsync();

var runner = scope.ServiceProvider.GetRequiredService<AppRunner>();
await runner.RunAsync();

public class AppRunner(IBookRepository books, IDatabaseSeeder seeder)
{
    public async Task RunAsync()
    {
        await seeder.SeedIfEmptyAsync();

        var list = await books.GetAllWithAuthorsAsync();

        foreach (var b in list)
        {
            Console.WriteLine($"{b.Title} - {b.Author.Name}");
        }

        if (list.Count == 0)
        {
            Console.WriteLine("No books found. Apply migrations and seed data as needed.");
        }
    }
}
