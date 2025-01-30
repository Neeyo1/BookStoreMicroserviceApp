namespace BookService.DTOs;

public class PublisherDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public string? PageUrl { get; set; }
}
