namespace BookService.Entities;

public class Publisher
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public string? PageUrl { get; set; }

    //Publisher - Book
    public ICollection<Book> Books { get; set; } = [];
}
