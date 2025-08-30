using EfCoreSamples;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Repositories;

// Educational notes:
// - This repository is intentionally small for learners to focus on EF patterns.
// - We return authors ordered by Name so the UI has a deterministic order.
// - For read-only screens you could append AsNoTracking() for a small perf win,
//   but keeping tracking on makes it easier to extend to editing scenarios.
public class AuthorRepository(SqlDbContext db)
{
    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Authors
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }
}
