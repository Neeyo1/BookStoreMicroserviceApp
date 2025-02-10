using MongoDB.Entities;
using PurchaseService.Entities;
using PurchaseService.Interfaces;

namespace PurchaseService.Data;

public class CartRepository : ICartRepository
{
    public async Task<Cart?> GetActiveCartByUsernameAsync(string username)
    {
        return await DB.Find<Cart>()
            .Match(x => x.Username == username && x.Status == "Active")
            .ExecuteFirstAsync();
    }
}
