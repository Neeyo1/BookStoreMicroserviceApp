using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using Nest;
using SearchService.Entities;
using SearchService.Helpers;
using StackExchange.Redis;

namespace SearchService.Consumers;

public class BookCreatedConsumer(IMapper mapper, ILogger<BookCreatedConsumer> logger,
    IConnectionMultiplexer redis, IElasticClient elasticClient) : IConsumer<BookCreated>
{
    public async Task Consume(ConsumeContext<BookCreated> context)
    {
        logger.LogInformation("------ Consuming BookCreated: {id} ------", context.Message.Id);
        var book = mapper.Map<Book>(context.Message);
        await book.SaveAsync();
        await elasticClient.IndexDocumentAsync(mapper.Map<BookES>(book));
        await RedisCacheHelper.RemoveKeysByPrefixAsync(redis, "BookStore_Items_");
    }
}
