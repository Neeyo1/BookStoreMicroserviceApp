using CartService.DTOs;
using CartService.Entities;

namespace CartService.Interfaces;

public interface ICartRepository
{
    void AddCart(Cart cart);
    Task<IEnumerable<CartDto>> GetCartsAsync(string username);
    Task<Cart?> GetCartByIdAsync(Guid cartId);
    Task<Cart?> GetCartWithDetailsByIdAsync(Guid cartId);
    Task<Cart?> GetActiveOrProceedingCartByUsernameAsync(string username);
    Task<BookCart?> GetBookCartByIdsAsync(Guid cartId, Guid bookId);
    Task<bool> Complete();
}
