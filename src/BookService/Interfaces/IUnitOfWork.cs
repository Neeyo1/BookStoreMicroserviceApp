namespace BookService.Interfaces;

public interface IUnitOfWork
{
    IAuthorRepository AuthorRepository { get; }
    IPublisherRepository PublisherRepository { get; }
    IBookRepository BookRepository { get; }
    IItemRepository ItemRepository { get; }
    Task<bool> Complete();
}
