namespace Contracts;

public class CartStatusChanged
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
}
