using CartService.Entities;

namespace CartService.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetBookByIdAsync(Guid bookId);
}
