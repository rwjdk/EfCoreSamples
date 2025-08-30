using EfCoreSamples.Data;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Repositories;

public class BookRepository(SqlDbContext db) : IBookRepository
{
    public async Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await db.Books
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}

