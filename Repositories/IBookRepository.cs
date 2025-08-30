using EfCoreSamples.Models;

namespace EfCoreSamples.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdWithAuthorAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(string title, string description, string authorName, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, string title, string description, string authorName, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Book>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}
