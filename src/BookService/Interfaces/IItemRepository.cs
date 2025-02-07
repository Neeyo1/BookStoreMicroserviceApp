using BookService.DTOs;
using BookService.Entities;

namespace BookService.Interfaces;

public interface IItemRepository
{
    void AddItem(Item item);
    Task<IEnumerable<ItemDto>> GetItemsAsync(Guid bookId);
    Task<Item?> GetItemByIdAsync(Guid itemId);
}
