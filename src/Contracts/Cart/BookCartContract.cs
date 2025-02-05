namespace Contracts.Cart;

public class BookCartContract
{
    public int Quantity { get; set; }
    public BookContract Book { get; set; } = null!;
}
