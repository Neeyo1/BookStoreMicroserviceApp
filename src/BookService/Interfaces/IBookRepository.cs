using BookService.DTOs;
using BookService.Entities;

namespace BookService.Interfaces;

public interface IBookRepository
{
    void AddBook(Book book);
    void DeleteBook(Book book);
    Task<IEnumerable<BookDto>> GetBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid bookId);
    Task<Book?> GetBookWithDetailsByIdAsync(Guid bookId);
}
