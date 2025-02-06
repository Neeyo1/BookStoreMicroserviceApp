using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using PurchaseService.Entities;

namespace PurchaseService.Consumers;

public class CartUpdatedConsumer(IMapper mapper, ILogger<CartUpdatedConsumer> logger) : IConsumer<CartUpdated>
{
    public async Task Consume(ConsumeContext<CartUpdated> context)
    {
        logger.LogInformation("------ Consuming CartUpdated: {id} ------", context.Message.Id);

        var cart = mapper.Map<Cart>(context.Message);
        var result = await DB.Update<Cart>()
            .Match(x => x.ID == cart.ID)
            .ModifyOnly(x => new
            {
                x.TotalPrice,
                x.UpdatedAt,
                x.Items
            }, cart)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(CartUpdated), 
                "Problem occured while updating cart in purchase database");
    }
}
