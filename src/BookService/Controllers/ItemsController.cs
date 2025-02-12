using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class ItemsController(IUnitOfWork unitOfWork, IMapper mapper,
    IPublishEndpoint publishEndpoint) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems(Guid bookId)
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
    public async Task<ActionResult> CreateItems(Guid bookId, int quantity)
    {
        var book = await unitOfWork.BookRepository.GetBookWithDetailsByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");

        for (int i = 0; i < quantity; i++)
        {
            var item = new Item
            {
                BookId = book.Id
            };
            unitOfWork.ItemRepository.AddItem(item);

            await publishEndpoint.Publish(mapper.Map<ItemCreated>(item));
        }

        await publishEndpoint.Publish(mapper.Map<BookUpdated>(book));

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to add items");
    }
}
