using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

public class ItemsController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ItemDto>> GetItems(Guid bookId)
    {
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");

        var items = await unitOfWork.ItemRepository.GetItemsAsync(bookId);
        
        return Ok(items);
    }
    
    [HttpGet("{itemId}")]
    public async Task<ActionResult<ItemDto>> GetItem(Guid itemId)
    {
        var item = await unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
        if (item == null) return NotFound();

        return Ok(mapper.Map<ItemDto>(item));
    }

    [HttpPut]
    public async Task<ActionResult<BookDto>> CreateBook(Guid bookId, int quantity)
    {
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");

        for (int i = 0; i < quantity; i++)
        {
            var item = new Item
            {
                BookId = book.Id
            };
            book.Items.Add(item);
        }

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to add items");
    }
}
