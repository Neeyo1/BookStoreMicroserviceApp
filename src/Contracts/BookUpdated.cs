namespace Contracts;

public class BookUpdated
{
    public Guid Id { get; set; }
    public int Price { get; set; }
    public int Items { get; set; }
    public required string ImageUrl { get; set; }
}
