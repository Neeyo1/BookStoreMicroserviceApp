using AutoMapper;
using BookService.Entities;
using BookService.Interfaces;
using Contracts;
using Contracts.Cart;
using MassTransit;

namespace BookService.Consumers;

public class ItemUpdatedConsumer(ILogger<ItemUpdatedConsumer> logger, IUnitOfWork unitOfWork,
    IMapper mapper, IPublishEndpoint publishEndpoint) : IConsumer<ItemUpdated>
{
    public async Task Consume(ConsumeContext<ItemUpdated> context)
    {
        logger.LogInformation("------ Consuming ItemUpdated: {id} ------", context.Message.Id);

        var item = await unitOfWork.ItemRepository.GetItemByIdAsync(context.Message.Id);
        if (item == null)
            throw new MessageException(typeof(ItemUpdated), 
                "Problem occured while searching for item in books database");

        mapper.Map(context.Message, item);

        var book = await unitOfWork.BookRepository.GetBookWithDetailsByIdAsync(item.BookId);
        if (book == null)
            throw new MessageException(typeof(ItemUpdated), 
                "Problem occured while searching for book in books database");

        await publishEndpoint.Publish(mapper.Map<BookUpdated>(book));

        if (!await unitOfWork.Complete())
            throw new MessageException(typeof(BookUpdated), 
                "Problem occured while updating book in carts database");
    }
}

