using BookService.DTOs;
using BookService.Entities;

namespace BookService.Interfaces;

public interface IItemRepository
{
    Task<IEnumerable<ItemDto>> GetItemsAsync(Guid bookId);
    Task<Item?> GetItemByIdAsync(Guid itemId);
}
