using CartService.Entities;

namespace CartService.Interfaces;

public interface IBookRepository
{
    void AddBook(Book book);
    void DeleteBook(Book book);
    Task<Book?> GetBookByIdAsync(Guid bookId);
}
