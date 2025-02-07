using PurchaseService.Entities;

namespace PurchaseService.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByUsernameAsync(string username);
}
