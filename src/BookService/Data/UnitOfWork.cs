using BookService.Interfaces;

namespace BookService.Data;

public class UnitOfWork(BookDbContext context, IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository) : IUnitOfWork
{
    public IAuthorRepository AuthorRepository => authorRepository;
    public IPublisherRepository PublisherRepository => publisherRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
