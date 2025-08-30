using EfCoreSamples.Models;

namespace EfCoreSamples.Repositories;

public interface IAuthorRepository
{
    // Placeholder for future author-specific methods
    Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

