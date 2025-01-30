using BookService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class BookDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Item> Items { get; set; }
}
