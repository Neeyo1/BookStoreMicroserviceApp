using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace PurchaseService.Entities;

public class Cart : Entity
{
    public required string Username { get; set; }
    public int TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [BsonElement("Items")]
    public List<BookCart> Items { get; set; } = new();
}
