namespace CartService.DTOs;

public class BookCartDto
{
    public int Quantity { get; set; }
    public BookDto Book { get; set; } = null!;
}
