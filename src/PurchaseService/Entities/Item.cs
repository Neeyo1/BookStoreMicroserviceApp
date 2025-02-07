using MongoDB.Entities;

namespace PurchaseService.Entities;

public class Item : Entity
{
    public ItemStatus Status { get; set; }
    public string? Buyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ReservedAt { get; set; }
    public DateTime? ReservedUntil { get; set; }
    public string? ReservedBy { get; set; }
    public required string BookId { get; set; }
}
