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

    public async Task<Book?> GetByIdWithAuthorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

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

    public async Task UpdateAsync(Guid id, string title, string description, string authorName, CancellationToken cancellationToken = default)
    {
        var book = await db.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
                   ?? throw new InvalidOperationException("Book not found");

        var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == authorName, cancellationToken)
                     ?? new Author { Id = Guid.NewGuid(), Name = authorName };
        if (db.Entry(author).State == EntityState.Detached && !(await db.Authors.AnyAsync(a => a.Id == author.Id, cancellationToken)))
        {
            db.Authors.Add(author);
        }

        book.Title = title;
        book.Description = description;
        book.AuthorId = author.Id;
        book.Author = author;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (book is null) return;
        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Book>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await db.Books
            .Where(b => b.AuthorId == authorId)
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}
