using AutoMapper;
using Contracts;
using Contracts.Cart;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using PurchaseService.Entities;
using PurchaseService.Interfaces;

namespace PurchaseService.Controllers;

[Authorize]
public class PurchasesController(IMapper mapper, IPurchaseRepository purchaseRepository,
    ICartRepository cartRepository, IPublishEndpoint publishEndpoint) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> Purchase()
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        var cart = await cartRepository.GetActiveCartByUsernameAsync(identity.Name);
        if (cart == null) return BadRequest("Failed to find active cart of given user");

        cart.Status = CartStatus.Proceeding;
        await DB.SaveAsync(cart);
        await publishEndpoint.Publish(mapper.Map<CartProceeding>(cart));

        try
        {
            var items = await purchaseRepository.GetItemsForReservation(cart);

            var timeNow = DateTime.UtcNow;

            foreach (var item in items)
            {
                item.Status = ItemStatus.Reserved;
                item.ReservedAt = timeNow;
                item.ReservedUntil = timeNow.AddMinutes(5);
                item.ReservedBy = cart.Username;

                await DB.SaveAsync(item);

                await publishEndpoint.Publish(mapper.Map<ItemUpdated>(item));
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
