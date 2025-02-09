using AutoMapper;
using AutoMapper.QueryableExtensions;
using CartService.DTOs;
using CartService.Entities;
using CartService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data;

public class CartRepository(CartDbContext context, IMapper mapper) : ICartRepository
{
    public void AddCart(Cart cart)
    {
        context.Carts.Add(cart);
    }

    public async Task<IEnumerable<CartDto>> GetCartsAsync(string username)
    {
        return await context.Carts
            .Where(x => x.Username == username)
            .Include(x => x.BookCarts)
            .ProjectTo<CartDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Cart?> GetCartByIdAsync(Guid cartId)
    {
        return await context.Carts
            .FindAsync(cartId);
    }

    public async Task<Cart?> GetCartWithDetailsByIdAsync(Guid cartId)
    {
        return await context.Carts
            .Include(x => x.BookCarts)
            .ThenInclude(x => x.Book)
            .FirstOrDefaultAsync(x => x.Id == cartId);
    }

    public async Task<Cart?> GetActiveOrProceedingCartByUsernameAsync(string username)
    {
        return await context.Carts
            .Include(x => x.BookCarts)
            .FirstOrDefaultAsync(x => x.Username == username
                && (x.Status == CartStatus.Active || x.Status == CartStatus.Proceeding));
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
