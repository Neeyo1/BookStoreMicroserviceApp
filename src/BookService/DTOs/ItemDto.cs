namespace BookService.DTOs;

public class ItemDto
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
    public string? Buyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ReservedAt { get; set; }
}
