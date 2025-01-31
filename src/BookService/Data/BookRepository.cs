using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class BookRepository(BookDbContext context, IMapper mapper) : IBookRepository
{
    public void AddBook(Book book)
    {
        context.Books.Add(book);
    }

    public void DeleteBook(Book book)
    {
        context.Books.Remove(book);
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync()
    {
        return await context.Books
            .ProjectTo<BookDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid bookId)
    {
        return await context.Books
            .FindAsync(bookId);
    }

    public async Task<Book?> GetBookWithDetailsByIdAsync(Guid bookId)
    {
        return await context.Books
            .Include(x => x.Author)
            .Include(x => x.Publisher)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == bookId);
    }
}
