using PurchaseService.Entities;

namespace PurchaseService.Interfaces;

public interface IPurchaseRepository
{
    Task<IEnumerable<Item>> GetItemsForReservation(Cart cart);
}
