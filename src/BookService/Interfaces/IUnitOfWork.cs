namespace BookService.Interfaces;

public interface IUnitOfWork
{
    IAuthorRepository AuthorRepository { get; }
    IPublisherRepository PublisherRepository { get; }
    Task<bool> Complete();
}
