using BookService.Interfaces;

namespace BookService.Data;

public class UnitOfWork(BookDbContext context, IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository, IBookRepository bookRepository,
    IItemRepository itemRepository) : IUnitOfWork
{
    public IAuthorRepository AuthorRepository => authorRepository;
    public IPublisherRepository PublisherRepository => publisherRepository;
    public IBookRepository BookRepository => bookRepository;
    public IItemRepository ItemRepository => itemRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
