using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Helpers;
using StackExchange.Redis;

namespace SearchService.Consumers;

public class BookCreatedConsumer(IMapper mapper, ILogger<BookCreatedConsumer> logger,
    IConnectionMultiplexer redis) : IConsumer<BookCreated>
{
    public async Task Consume(ConsumeContext<BookCreated> context)
    {
        logger.LogInformation("------ Consuming BookCreated: {id} ------", context.Message.Id);
        var book = mapper.Map<Book>(context.Message);
        await book.SaveAsync();
        await RedisCacheHelper.RemoveKeysByPrefixAsync(redis, "BookStore_Items_");
    }
}
