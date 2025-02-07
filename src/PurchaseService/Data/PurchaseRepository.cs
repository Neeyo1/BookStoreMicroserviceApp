using MongoDB.Entities;
using PurchaseService.Entities;
using PurchaseService.Interfaces;

namespace PurchaseService.Data;

public class PurchaseRepository() : IPurchaseRepository
{
    public async Task<IEnumerable<Item>> GetItemsForReservation(Cart cart)
    {
        var itemsForReservation = new List<Item>();

        var items = cart.Items;
        foreach (var item in items)
        {
            var newItems = await DB.Find<Item>()
                .Match(x => x.BookId == item.Book.ID && x.Status == ItemStatus.Avaiable)
                .Limit(item.Quantity)
                .ExecuteAsync();

            if (newItems.Count < item.Quantity)
            {
                throw new Exception($"Not enough items for book of id {item.Book.ID}");
            }

            itemsForReservation.AddRange(newItems);
        }

        return itemsForReservation;
    }
}
