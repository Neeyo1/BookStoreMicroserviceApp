namespace CartService.DTOs;

public class CartDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public int TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<BookCartDto> Items { get; set; } = [];
}
