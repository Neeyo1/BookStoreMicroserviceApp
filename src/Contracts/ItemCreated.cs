namespace Contracts;

public class ItemCreated
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
    public string? Buyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ReservedAt { get; set; }
    public DateTime? ReservedUntil { get; set; }
    public string? ReservedBy { get; set; }
    public Guid BookId { get; set; }
}
