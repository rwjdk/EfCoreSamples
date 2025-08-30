using Microsoft.EntityFrameworkCore;
using EfCoreSamples;
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

// EF Core: read the connection string (for UseSqlServer below)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// EF Core: register the DbContext in DI and select the SQL Server provider
// - AddDbContext adds SqlDbContext with a scoped lifetime
// - UseSqlServer wires up the SQL Server EF Core provider using the connection string above
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(connectionString));

// App services
builder.Services.AddScoped<AppRunner>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<AuthorRepository>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<ConsoleUi>();

var host = builder.Build();

// Run a scoped operation
// EF Core: create a scope (DbContext is scoped) and resolve it
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SqlDbContext>();

// EF Core: apply pending migrations (creates the database if it doesn't exist)
// - Prefer Migrate() over EnsureCreated() when using migrations
await db.Database.MigrateAsync();

var runner = scope.ServiceProvider.GetRequiredService<AppRunner>();
await runner.RunAsync();

public class AppRunner(ConsoleUi ui, DatabaseSeeder seeder)
{
    public async Task RunAsync()
    {
        await seeder.SeedIfEmptyAsync();

        await ui.RunAsync();
    }
}
