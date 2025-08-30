using EfCoreSamples.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSamples.Data;

public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
}
