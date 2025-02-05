using CartService.DTOs;
using CartService.Entities;

namespace CartService.Interfaces;

public interface ICartRepository
{
    void AddCart(Cart cart);
    Task<IEnumerable<CartDto>> GetCartsAsync(string username);
    Task<Cart?> GetCartByIdAsync(Guid cartId);
    Task<Cart?> GetCartWithDetailsByIdAsync(Guid cartId);
    Task<Cart?> GetCartByUsernameAsync(string username);
    Task<bool> Complete();
}
