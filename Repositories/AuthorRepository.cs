using EfCoreSamples.Data;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Repositories;

public class AuthorRepository(SqlDbContext db) : IAuthorRepository
{
    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await db.Authors.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<Author> GetOrCreateByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var author = await GetByNameAsync(name, cancellationToken);
        if (author is not null) return author;
        author = new Author { Id = Guid.NewGuid(), Name = name };
        db.Authors.Add(author);
        await db.SaveChangesAsync(cancellationToken);
        return author;
    }

    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Authors.OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }
}
