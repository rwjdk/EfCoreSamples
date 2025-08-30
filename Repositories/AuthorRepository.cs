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
}

