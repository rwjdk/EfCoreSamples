namespace EfCoreSamples.Services;

public interface IDatabaseSeeder
{
    Task SeedIfEmptyAsync(CancellationToken cancellationToken = default);
}
