using AutoMapper;
using Contracts;
using MassTransit;
using SearchService.Entities;
using SearchService.Helpers;
using SearchService.Interfaces;
using StackExchange.Redis;

namespace SearchService.Consumers;

public class BookUpdatedConsumer(IMapper mapper, ISearchRepository searchRepository,
    ILogger<BookUpdatedConsumer> logger, IConnectionMultiplexer redis) : IConsumer<BookUpdated>
{
    public async Task Consume(ConsumeContext<BookUpdated> context)
    {
        logger.LogInformation("------ Consuming BookUpdated: {id} ------", context.Message.Id);
        var book = mapper.Map<Book>(context.Message);
        var result = await searchRepository.UpdateBook(book);
        if (result.IsAcknowledged)
        {
            await RedisCacheHelper.RemoveKeysByPrefixAsync(redis, "BookStore_Items_");
        }
        else
        {
            throw new MessageException(typeof(BookUpdated), 
                "Problem occured while updating book in search database");
        }
    }
}
