using MongoDB.Entities;

namespace PurchaseService.Entities;

public class BookCart : Entity
{
    public int Quantity { get; set; }
    public Book Book { get; set; } = null!;
}
