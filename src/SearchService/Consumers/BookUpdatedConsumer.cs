using AutoMapper;
using Contracts;
using MassTransit;
using SearchService.Entities;
using SearchService.Interfaces;

namespace SearchService.Consumers;

public class BookUpdatedConsumer(IMapper mapper, ISearchRepository searchRepository,
    ILogger<BookUpdatedConsumer> logger) : IConsumer<BookUpdated>
{
    public async Task Consume(ConsumeContext<BookUpdated> context)
    {
        logger.LogInformation("------ Consuming BookUpdated: {id} ------", context.Message.Id);
        var book = mapper.Map<Book>(context.Message);
        var result = await searchRepository.UpdateBook(book);
        if (!result.IsAcknowledged)
            throw new MessageException(typeof(BookUpdated), 
                "Problem occured while updating book in search database");
    }
}
