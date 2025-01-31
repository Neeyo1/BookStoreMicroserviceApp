namespace BookService.DTOs;

public class BookUpdateDto
{
    public string? Name { get; set; }
    public int? Year { get; set; }
    public string? ImageUrl { get; set; }
    public int? Price { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? PublisherId { get; set; }
}
