using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class ItemRepository(BookDbContext context, IMapper mapper) : IItemRepository
{
    public async Task<IEnumerable<ItemDto>> GetItemsAsync(Guid bookId)
    {
        return await context.Items
            .Where(x => x.BookId == bookId)
            .ProjectTo<ItemDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Item?> GetItemByIdAsync(Guid itemId)
    {
        return await context.Items
            .FindAsync(itemId);
    }
}
