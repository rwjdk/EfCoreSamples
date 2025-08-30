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

        var entries = new (string Title, string Author, string Description)[]
        {
            ("Pride and Prejudice", "Jane Austen", "A classic novel of manners and romance in Regency England."),
            ("Sense and Sensibility", "Jane Austen", "Dashwood sisters navigate love, sense, and sensibility."),
            ("1984", "George Orwell", "Dystopian surveillance state, Big Brother, and thoughtcrime."),
            ("Animal Farm", "George Orwell", "Political satire about a farm revolution gone wrong."),
            ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "A boy discovers he is a wizard and attends Hogwarts."),
            ("The Hobbit", "J.R.R. Tolkien", "Bilbo Baggins embarks on an unexpected journey."),
            ("The Lord of the Rings", "J.R.R. Tolkien", "Epic quest to destroy the One Ring."),
            ("The Great Gatsby", "F. Scott Fitzgerald", "Jazz Age tale of wealth, love, and illusion."),
            ("To Kill a Mockingbird", "Harper Lee", "A child's view on racial injustice in the Deep South."),
            ("Moby-Dick", "Herman Melville", "Captain Ahab's obsessive quest for the white whale."),
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

            var book = new Book { Id = Guid.NewGuid(), Title = e.Title, Description = e.Description, AuthorId = author.Id, Author = author };
            db.Books.Add(book);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
