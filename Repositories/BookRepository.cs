using EfCoreSamples;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Repositories;

// Educational notes:
// - Repositories wrap EF Core so callers don't think in terms of DbContext/DbSet.
// - We inject the DbContext via DI; its default lifetime is Scoped, which
//   matches a single unit of work (here: one console operation cycle).
// - All reads below eager-load the Author to avoid the N+1 problem
//   (multiple queries triggered by lazy loads one-by-one).
public class BookRepository(SqlDbContext db)
{
    // Returns books with their Author loaded in a single SQL query.
    // Include tells EF Core to produce a JOIN so the navigation is populated
    // without extra roundtrips. OrderBy keeps results stable for the UI.
    public async Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await db.Books
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    // Creation pattern:
    // - Look up an existing Author by name to avoid duplicates.
    // - If not found, create and attach a new Author (DbContext starts tracking it).
    // - Create a Book that references the Author by FK and navigation.
    // - SaveChanges once to persist both rows in a single transaction.
    // Notes:
    // - We assign Guid keys in code; EF could also generate GUIDs server-side,
    //   but assigning here makes intent explicit for learners.
    // - We keep tracking enabled (no AsNoTracking) so additions/updates flow naturally.
    public async Task<Book> CreateAsync(string title, string description, string authorName, CancellationToken cancellationToken = default)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == authorName, cancellationToken)
                     ?? new Author { Id = Guid.NewGuid(), Name = authorName };

        if (author.Id == Guid.Empty) author.Id = Guid.NewGuid();
        if (db.Entry(author).State == EntityState.Detached && !(await db.Authors.AnyAsync(a => a.Id == author.Id, cancellationToken)))
        {
            db.Authors.Add(author);
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            AuthorId = author.Id,
            Author = author
        };
        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);
        return book;
    }

    // Delete pattern:
    // - FirstOrDefaultAsync loads the entity for the given key.
    // - If found, remove it and SaveChanges. If not found, do nothing.
    // - For production apps consider soft-delete or concurrency checks.
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (book is null) return;
        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);
    }

    // Filtered read:
    // - WHERE on the FK (AuthorId) is efficient and uses the index added by EF.
    // - Include keeps the Author navigation populated for the UI.
    public async Task<List<Book>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await db.Books
            .Where(b => b.AuthorId == authorId)
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}
