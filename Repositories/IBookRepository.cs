using EfCoreSamples.Models;

namespace EfCoreSamples.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
}

