namespace CartService.Entities;

public class Book
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public required string ImageUrl { get; set; }
    public int Price { get; set; }
    public int Items { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorFirstName { get; set; }
    public string? AuthorLastName { get; set; }
    public string? AuthorAlias { get; set; }
    public Guid PublisherId { get; set; }
    public required string PublisherName { get; set; }
    public required string PublisherCountry { get; set; }
    public required string PublisherCity { get; set; }
    public required string PublisherAddress { get; set; }
    public required string PublisherPhoneNumber { get; set; }
    public string? PublisherPageUrl { get; set; }

    // Book - Cart
    public ICollection<BookCart> BookCarts { get; set; } = [];
}
