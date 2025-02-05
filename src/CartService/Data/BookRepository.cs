using CartService.Entities;
using CartService.Interfaces;

namespace CartService.Data;

public class BookRepository(CartDbContext context) : IBookRepository
{
    public void AddBook(Book book)
    {
        context.Books.Add(book);
    }

    public void DeleteBook(Book book)
    {
        context.Books.Remove(book);
    }
    
    public async Task<Book?> GetBookByIdAsync(Guid bookId)
    {
        return await context.Books
            .FindAsync(bookId);
    }
}
