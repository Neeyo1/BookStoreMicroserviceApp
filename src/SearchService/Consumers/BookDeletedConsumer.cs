using Contracts;
using MassTransit;
using SearchService.Helpers;
using SearchService.Interfaces;
using StackExchange.Redis;

namespace SearchService.Consumers;

public class BookDeletedConsumer(ISearchRepository searchRepository, ILogger<BookDeletedConsumer> logger,
    IConnectionMultiplexer redis) : IConsumer<BookDeleted>
{
    public async Task Consume(ConsumeContext<BookDeleted> context)
    {
        logger.LogInformation("------ Consuming BookDeleted: {id} ------", context.Message.Id);
        var result = await searchRepository.DeleteBook(context.Message.Id);
        if (result.IsAcknowledged)
        {
            await RedisCacheHelper.RemoveKeysByPrefixAsync(redis, "BookStore_Items_");
        }
        else
        {
            throw new MessageException(typeof(BookDeleted), 
                "Problem occured while deleting book in search database");
        }
    }
}