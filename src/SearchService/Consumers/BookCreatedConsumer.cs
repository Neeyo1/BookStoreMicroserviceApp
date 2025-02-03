using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class BookCreatedConsumer(IMapper mapper, ILogger<BookCreatedConsumer> logger) : IConsumer<BookCreated>
{
    public async Task Consume(ConsumeContext<BookCreated> context)
    {
        logger.LogInformation("------ Consuming BookCreated: {id} ------", context.Message.Id);
        var book = mapper.Map<Book>(context.Message);
        await book.SaveAsync();
    }
}
