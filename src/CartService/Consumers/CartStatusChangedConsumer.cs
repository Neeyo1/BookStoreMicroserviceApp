using CartService.Entities;
using CartService.Interfaces;
using Contracts;
using MassTransit;

namespace CartService.Consumers;

public class CartStatusChangedConsumer(ILogger<CartStatusChangedConsumer> logger,
    ICartRepository cartRepository) : IConsumer<CartStatusChanged>
{
    public async Task Consume(ConsumeContext<CartStatusChanged> context)
    {
        logger.LogInformation("------ Consuming CartStatusChanged: {id} ------", context.Message.Id);

        var cart = await cartRepository.GetCartByIdAsync(context.Message.Id);
        if (cart == null)
            throw new MessageException(typeof(CartStatusChanged), 
                "Problem occured while searching for cart in carts database");

        cart.Status = context.Message.Status switch
        {
            "Active" => CartStatus.Active,
            "Proceeding" => CartStatus.Proceeding,
            "Finished" => CartStatus.Finished,
            _ => throw new MessageException(typeof(CartStatusChanged),
                                "Unexpected cart status provided by purchase service"),
        };
        if (!await cartRepository.Complete())
            throw new MessageException(typeof(CartStatusChanged), 
                "Problem occured while updating cart in carts database");
    }
}
