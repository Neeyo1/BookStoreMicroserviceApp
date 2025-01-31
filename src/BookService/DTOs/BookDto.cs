namespace BookService.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public required string ImageUrl { get; set; }
    public int Price { get; set; }
    public int Items { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public PublisherDto Publisher { get; set; } = null!;
}
