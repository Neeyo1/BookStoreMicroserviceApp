using Contracts.Cart;

namespace Contracts;

public class CartCreated
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public int TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public required string Status { get; set; }
    public IEnumerable<BookCartContract> Items { get; set; } = [];
}
