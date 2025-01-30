namespace BookService.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Alias { get; set; }

    //Author - Book
    public ICollection<Book> Books { get; set; } = [];
}
