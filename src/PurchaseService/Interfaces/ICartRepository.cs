using PurchaseService.Entities;

namespace PurchaseService.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetActiveCartByUsernameAsync(string username);
}
