using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using PurchaseService.Entities;

namespace PurchaseService.Consumers;

public class CartCreatedConsumer(IMapper mapper, ILogger<CartCreatedConsumer> logger) : IConsumer<CartCreated>
{
    public async Task Consume(ConsumeContext<CartCreated> context)
    {
        logger.LogInformation("------ Consuming CartCreated: {id} ------", context.Message.Id);
        var cart = mapper.Map<Cart>(context.Message);
        await cart.SaveAsync();
    }
}
