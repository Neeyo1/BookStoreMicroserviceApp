namespace CartService.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public int TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Cart - Book
    public ICollection<BookCart> BookCarts { get; set; } = [];
}
