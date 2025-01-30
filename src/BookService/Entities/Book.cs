namespace BookService.Entities;

public class Book
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public required string ImageUrl { get; set; }
    public int Price { get; set; }

    //Book - Author
    public Guid AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    //Book - Publisher
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    //Book - Item
    public ICollection<Item> Items { get; set; } = [];
}