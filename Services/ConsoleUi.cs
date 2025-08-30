using EfCoreSamples.Repositories;

namespace EfCoreSamples.Services;

public class ConsoleUi(IBookRepository books, IAuthorRepository authors)
{
    public async Task RunAsync()
    {
        await ShowMenuAsync();
    }

    private async Task ShowMenuAsync()
    {
        var options = new[]
        {
            "List books",
            "Add book",
            "Remove book",
            "List authors",
            "Exit"
        };

        while (true)
        {
            var idx = await SelectFromListAsync(options, s => s, "==== Book Library ====");
            if (idx < 0 || idx == 4) return; // Esc/backspace or Exit

            switch (idx)
            {
                case 0:
                    await ListBooksInteractiveAsync();
                    break;
                case 1:
                    await AddBookAsync();
                    break;
                case 2:
                    await RemoveBookInteractiveAsync();
                    break;
                case 3:
                    await ListAuthorsInteractiveAsync();
                    break;
            }
        }
    }

    private async Task AddBookAsync()
    {
        Console.Write("Title: ");
        var title = ReadNonEmpty();
        Console.Write("Author name: ");
        var author = ReadNonEmpty();
        Console.Write("Description: ");
        var description = ReadNonEmpty();

        var created = await books.CreateAsync(title, description, author);
        Console.WriteLine($"Added: {created.Title} - {created.Author.Name}");
    }

    private async Task ListBooksInteractiveAsync()
    {
        var list = await books.GetAllWithAuthorsAsync();
        if (list.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }
        var idx = await SelectFromListAsync(list, b => $"{b.Title} - {b.Author.Name}", "Books (Enter=details, Esc=back)");
        if (idx < 0) return;
        var bsel = list[idx];
        Console.Clear();
        Console.WriteLine($"Title: {bsel.Title}");
        Console.WriteLine($"Author: {bsel.Author.Name}");
        Console.WriteLine("Description:");
        Console.WriteLine(bsel.Description);
        Console.WriteLine();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }

    private async Task RemoveBookInteractiveAsync()
    {
        var list = await books.GetAllWithAuthorsAsync();
        if (list.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }
        var idx = await SelectFromListAsync(list, b => $"{b.Title} - {b.Author.Name}", "Remove Book (Enter=delete, Esc=back)");
        if (idx < 0) return;
        var bsel = list[idx];
        Console.Write($"Delete '{bsel.Title}' by {bsel.Author.Name}? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase)) return;
        await books.DeleteAsync(bsel.Id);
        Console.WriteLine("Deleted.");
    }

    private async Task ListAuthorsInteractiveAsync()
    {
        var list = await authors.GetAllAsync();
        if (list.Count == 0)
        {
            Console.WriteLine("No authors found.");
            return;
        }
        var idx = await SelectFromListAsync(list, a => a.Name, "Authors (Enter=list books, Esc=back)");
        if (idx < 0) return;
        var author = list[idx];
        var abooks = await books.GetByAuthorIdAsync(author.Id);
        Console.Clear();
        Console.WriteLine($"Books by {author.Name}:");
        Console.WriteLine();
        if (abooks.Count == 0)
        {
            Console.WriteLine("No books for this author.");
        }
        else
        {
            foreach (var b in abooks)
            {
                Console.WriteLine($"- {b.Title}");
                Console.WriteLine($"  {Truncate(b.Description, 120)}");
            }
        }
        Console.WriteLine();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }

    private static Task<int> SelectFromListAsync<T>(IList<T> items, Func<T, string> render, string header)
    {
        if (items.Count == 0) return Task.FromResult(-1);
        var index = 0;
        while (true)
        {
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(header))
            {
                Console.WriteLine(header);
                Console.WriteLine();
            }
            for (int i = 0; i < items.Count; i++)
            {
                var prefix = i == index ? ">" : " ";
                Console.WriteLine($"{prefix} {render(items[i])}");
            }
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    index = (index - 1 + items.Count) % items.Count;
                    break;
                case ConsoleKey.DownArrow:
                    index = (index + 1) % items.Count;
                    break;
                case ConsoleKey.PageUp:
                    index = Math.Max(0, index - 10);
                    break;
                case ConsoleKey.PageDown:
                    index = Math.Min(items.Count - 1, index + 10);
                    break;
                case ConsoleKey.Enter:
                    return Task.FromResult(index);
                case ConsoleKey.Escape:
                case ConsoleKey.Backspace:
                    return Task.FromResult(-1);
            }
        }
    }

    private static string ReadNonEmpty()
    {
        while (true)
        {
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.Write("Please enter a value: ");
        }
    }

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..(max - 3)] + "...";
    }
}

