using Contracts.Cart;

namespace Contracts;

public class CartUpdated
{
    public Guid Id { get; set; }
    public int TotalPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<BookCartContract> Items { get; set; } = [];
}
