using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

public class BooksController(IUnitOfWork unitOfWork, IMapper mapper,
    IPublishEndpoint publishEndpoint) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await unitOfWork.BookRepository.GetBooksAsync();
        
        return Ok(books);
    }

    [HttpGet("{bookId}")]
    public async Task<ActionResult<BookDto>> GetBook(Guid bookId)
    {
        var book = await unitOfWork.BookRepository.GetBookWithDetailsByIdAsync(bookId);
        if (book == null) return NotFound();

        return Ok(mapper.Map<BookDto>(book));
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(BookCreateDto bookCreateDto)
    {
        var author = await unitOfWork.AuthorRepository.GetAuthorByIdAsync(bookCreateDto.AuthorId);
        if (author == null) return BadRequest("Failed to find author of given id");

        var publisher = await unitOfWork.PublisherRepository.GetPublisherByIdAsync(bookCreateDto.PublisherId);
        if (publisher == null) return BadRequest("Failed to find publisher of given id");

        var book = mapper.Map<Book>(bookCreateDto);

        unitOfWork.BookRepository.AddBook(book);

        var newBook = mapper.Map<BookDto>(book);

        await publishEndpoint.Publish(mapper.Map<BookCreated>(newBook));

        if (await unitOfWork.Complete())
            return CreatedAtAction(nameof(GetBook), new {bookId = book.Id}, newBook);
        return BadRequest("Failed to create book");
    }

    [HttpPut("{bookId}")]
    public async Task<ActionResult> UpdatetBook(BookUpdateDto bookUpdateDto, Guid bookId)
    {
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book");

        book.ImageUrl = bookUpdateDto.ImageUrl ?? book.ImageUrl;
        book.Price = bookUpdateDto.Price ?? book.Price;

        await publishEndpoint.Publish(mapper.Map<BookUpdated>(book));

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update book");
    }

    [HttpDelete("{bookId}")]
    public async Task<ActionResult> DeleteBook(Guid bookId)
    {  
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book");

        unitOfWork.BookRepository.DeleteBook(book);

        await publishEndpoint.Publish<BookDeleted>(new {Id = book.Id});

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to delete book");
    }

    [HttpGet("{bookId}/items")]
    public async Task<ActionResult<ItemDto>> GetItems(Guid bookId)
    {
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");
        
        var items = await unitOfWork.ItemRepository.GetItemsAsync(bookId);
        
        return Ok(items);
    }
}
