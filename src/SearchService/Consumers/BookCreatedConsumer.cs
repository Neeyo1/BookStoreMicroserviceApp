using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class BookCreatedConsumer(IMapper mapper) : IConsumer<BookCreated>
{
    public async Task Consume(ConsumeContext<BookCreated> context)
    {
        Console.WriteLine("------ Consuming BookCreated: " + context.Message.Id + " ------");
        var book = mapper.Map<Book>(context.Message);
        await book.SaveAsync();
    }
}
