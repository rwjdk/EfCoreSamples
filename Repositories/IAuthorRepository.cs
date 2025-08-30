using EfCoreSamples.Models;

namespace EfCoreSamples.Repositories;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default);
}
