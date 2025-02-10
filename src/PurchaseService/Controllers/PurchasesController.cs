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
    public async Task<ActionResult> Purchase(bool paymentResult)
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        var cart = await cartRepository.GetActiveCartByUsernameAsync(identity.Name);
        if (cart == null) return BadRequest("Failed to find active cart of given user");

        try
        {
            var items = await purchaseRepository.GetItemsForReservation(cart);

            cart.Status = "Proceeding";
            await DB.SaveAsync(cart);
            await publishEndpoint.Publish(mapper.Map<CartStatusChanged>(cart));

            await UpdateItemsToReserved(items, cart.Username);

            await Task.Delay(5000); // Wait for payment process

            if (paymentResult)
            {
                await UpdateItemsToSold(items, cart.Username);

                cart.Status = "Finished";
                await DB.SaveAsync(cart);
                await publishEndpoint.Publish(mapper.Map<CartStatusChanged>(cart));

                return Ok("Paument proceed successfully");
            }
            else
            {
                await UpdateItemsToAvaiable(items);

                cart.Status = "Active";
                await DB.SaveAsync(cart);
                await publishEndpoint.Publish(mapper.Map<CartStatusChanged>(cart));

                return BadRequest("Payment failed");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task UpdateItemsToReserved(IEnumerable<Item> items, string username)
    {
        var timeNow = DateTime.UtcNow;

        foreach (var item in items)
        {
            item.Status = ItemStatus.Reserved;
            item.ReservedAt = timeNow;
            item.ReservedUntil = timeNow.AddMinutes(5);
            item.ReservedBy = username;

            await DB.SaveAsync(item);

            await publishEndpoint.Publish(mapper.Map<ItemUpdated>(item));
        }
    }

    private async Task UpdateItemsToSold(IEnumerable<Item> items, string username)
    {
        foreach (var item in items)
        {
            item.Status = ItemStatus.Sold;
            item.Buyer = username;

            await DB.SaveAsync(item);

            await publishEndpoint.Publish(mapper.Map<ItemUpdated>(item));
        }
    }

    private async Task UpdateItemsToAvaiable(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            item.Status = ItemStatus.Avaiable;
            item.ReservedAt = null;
            item.ReservedUntil = null;
            item.ReservedBy = null;

            await DB.SaveAsync(item);

            await publishEndpoint.Publish(mapper.Map<ItemUpdated>(item));
        }
    }
}
