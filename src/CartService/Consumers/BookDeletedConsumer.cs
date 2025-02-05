using CartService.Interfaces;
using Contracts;
using MassTransit;

namespace CartService.Consumers;

public class BookDeletedConsumer(ILogger<BookDeletedConsumer> logger, IBookRepository bookRepository,
    ICartRepository cartRepository) : IConsumer<BookDeleted>
{
    public async Task Consume(ConsumeContext<BookDeleted> context)
    {
        logger.LogInformation("------ Consuming BookDeleted: {id} ------", context.Message.Id);

        var book = await bookRepository.GetBookByIdAsync(context.Message.Id);
        if (book == null)
            throw new MessageException(typeof(BookDeleted), 
                "Problem occured while searching for book in carts database");
    
        bookRepository.DeleteBook(book);

        if (!await cartRepository.Complete())
            throw new MessageException(typeof(BookCreated), 
                "Problem occured while deleting book in carts database");
    }
}
