using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using PurchaseService.Entities;

namespace PurchaseService.Consumers;

public class ItemCreatedConsumer(IMapper mapper, ILogger<ItemCreatedConsumer> logger) : IConsumer<ItemCreated>
{
    public async Task Consume(ConsumeContext<ItemCreated> context)
    {
        logger.LogInformation("------ Consuming ItemCreated: {id} ------", context.Message.Id);
        var item = mapper.Map<Item>(context.Message);
        await item.SaveAsync();
    }
}
