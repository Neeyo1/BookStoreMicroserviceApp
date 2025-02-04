using CartService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data;

public class CartDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<BookCart> BookCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
    }
}
