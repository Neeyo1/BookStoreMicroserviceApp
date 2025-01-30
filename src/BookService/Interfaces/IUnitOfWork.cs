namespace BookService.Interfaces;

public interface IUnitOfWork
{
    IAuthorRepository AuthorRepository {get;}
    Task<bool> Complete();
}
