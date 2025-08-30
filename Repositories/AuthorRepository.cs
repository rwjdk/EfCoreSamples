using EfCoreSamples.Data;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Repositories;

public class AuthorRepository(SqlDbContext db) : IAuthorRepository
{
    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Authors.OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }
}
