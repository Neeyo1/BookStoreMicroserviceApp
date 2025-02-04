namespace CartService.Entities;

public class BookCart
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }

    // BookCart - Book
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    // BookCart - Cart
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;
}
