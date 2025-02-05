using CartService.Entities;
using CartService.Interfaces;

namespace CartService.Data;

public class BookRepository(CartDbContext context) : IBookRepository
{
    public async Task<Book?> GetBookByIdAsync(Guid bookId)
    {
        return await context.Books
            .FindAsync(bookId);
    }
}
