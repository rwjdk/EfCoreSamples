using System.ComponentModel.DataAnnotations;

namespace EfCoreSamples.Models;

public class Book
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(300)]
    public required string Title { get; set; }

    // Foreign key
    public Guid AuthorId { get; set; }

    // Navigation
    [Required]
    public required Author Author { get; set; }
}
