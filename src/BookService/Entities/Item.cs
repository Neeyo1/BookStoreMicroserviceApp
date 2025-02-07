namespace BookService.Entities;

public class Item
{
    public Guid Id { get; set; }
    public Status Status { get; set; } = Status.Avaiable;
    public string? Buyer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReservedAt { get; set; }
    public DateTime? ReservedUntil { get; set; }
    public string? ReservedBy { get; set; }

    //Item - Book
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}
