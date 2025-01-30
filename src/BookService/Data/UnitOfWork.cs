using BookService.Interfaces;

namespace BookService.Data;

public class UnitOfWork(BookDbContext context, IAuthorRepository authorRepository) : IUnitOfWork
{
    public IAuthorRepository AuthorRepository => authorRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
