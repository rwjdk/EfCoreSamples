using EfCoreSamples.Models;

namespace EfCoreSamples.Repositories;

public interface IAuthorRepository
{
    // Placeholder for future author-specific methods
    Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Author> GetOrCreateByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default);
}
