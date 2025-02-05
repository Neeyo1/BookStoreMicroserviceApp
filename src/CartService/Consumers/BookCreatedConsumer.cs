using AutoMapper;
using CartService.Entities;
using CartService.Interfaces;
using Contracts;
using MassTransit;

namespace CartService.Consumers;

public class BookCreatedConsumer(IMapper mapper, ILogger<BookCreatedConsumer> logger,
    IBookRepository bookRepository, ICartRepository cartRepository) : IConsumer<BookCreated>
{
    public async Task Consume(ConsumeContext<BookCreated> context)
    {
        logger.LogInformation("------ Consuming BookCreated: {id} ------", context.Message.Id);

        var book = mapper.Map<Book>(context.Message);
        bookRepository.AddBook(book);

        if (!await cartRepository.Complete())
            throw new MessageException(typeof(BookCreated), 
                "Problem occured while adding book in carts database");
    }
}
