using System.ComponentModel.DataAnnotations;

namespace EfCoreSamples.Models;

public class Author
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
