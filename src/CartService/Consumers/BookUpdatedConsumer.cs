using AutoMapper;
using CartService.Interfaces;
using Contracts;
using MassTransit;

namespace CartService.Consumers;

public class BookUpdatedConsumer(ILogger<BookUpdatedConsumer> logger, IBookRepository bookRepository,
    ICartRepository cartRepository) : IConsumer<BookUpdated>
{
    public async Task Consume(ConsumeContext<BookUpdated> context)
    {
        logger.LogInformation("------ Consuming BookUpdated: {id} ------", context.Message.Id);

        var book = await bookRepository.GetBookByIdAsync(context.Message.Id);
        if (book == null)
            throw new MessageException(typeof(BookUpdated), 
                "Problem occured while searching for book in carts database");

        book.Price = context.Message.Price;
        book.Items = context.Message.Items;
        book.ImageUrl = context.Message.ImageUrl;

        if (!await cartRepository.Complete())
            throw new MessageException(typeof(BookUpdated), 
                "Problem occured while updating book in carts database");
    }
}
