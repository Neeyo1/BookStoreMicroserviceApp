using Contracts;
using MassTransit;
using SearchService.Interfaces;

namespace SearchService.Consumers;

public class BookDeletedConsumer(ISearchRepository searchRepository,
    ILogger<BookDeletedConsumer> logger) : IConsumer<BookDeleted>
{
    public async Task Consume(ConsumeContext<BookDeleted> context)
    {
        logger.LogInformation("------ Consuming BookDeleted: {id} ------", context.Message.Id);
        var result = await searchRepository.DeleteBook(context.Message.Id);
        if (!result.IsAcknowledged)
            throw new MessageException(typeof(BookDeleted), 
                "Problem occured while deleting book in search database");
    }
}