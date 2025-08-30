using EfCoreSamples.Data;
using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Services;

public class DatabaseSeeder(SqlDbContext db) : IDatabaseSeeder
{
    public async Task SeedIfEmptyAsync(CancellationToken cancellationToken = default)
    {
        if (await db.Books.AnyAsync(cancellationToken) || await db.Authors.AnyAsync(cancellationToken))
            return;

        var entries = new (string Title, string Author)[]
        {
            ("Pride and Prejudice", "Jane Austen"),
            ("Sense and Sensibility", "Jane Austen"),
            ("1984", "George Orwell"),
            ("Animal Farm", "George Orwell"),
            ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling"),
            ("The Hobbit", "J.R.R. Tolkien"),
            ("The Lord of the Rings", "J.R.R. Tolkien"),
            ("The Great Gatsby", "F. Scott Fitzgerald"),
            ("To Kill a Mockingbird", "Harper Lee"),
            ("Moby-Dick", "Herman Melville"),
        };

        var rnd = new Random();
        var shuffled = entries.OrderBy(_ => rnd.Next()).ToList();

        var authorsByName = new Dictionary<string, Author>(StringComparer.OrdinalIgnoreCase);

        foreach (var e in shuffled)
        {
            if (!authorsByName.TryGetValue(e.Author, out var author))
            {
                author = new Author { Id = Guid.NewGuid(), Name = e.Author };
                authorsByName[e.Author] = author;
                db.Authors.Add(author);
            }

            var book = new Book { Id = Guid.NewGuid(), Title = e.Title, AuthorId = author.Id, Author = author };
            db.Books.Add(book);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

