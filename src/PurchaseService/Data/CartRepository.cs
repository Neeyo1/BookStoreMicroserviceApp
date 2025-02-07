using MongoDB.Entities;
using PurchaseService.Entities;
using PurchaseService.Interfaces;

namespace PurchaseService.Data;

public class CartRepository : ICartRepository
{
    public async Task<Cart?> GetCartByUsernameAsync(string username)
    {
        return await DB.Find<Cart>()
            .Match(x => x.Username == username)
            .ExecuteFirstAsync();
    }
}
