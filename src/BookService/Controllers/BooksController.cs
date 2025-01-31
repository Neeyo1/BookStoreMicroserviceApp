using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

public class BooksController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
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

        if (await unitOfWork.Complete())
            return CreatedAtAction(nameof(GetBook), new {bookId = book.Id}, newBook);
            
        return BadRequest("Failed to create book");
    }

    [HttpPut("{bookId}")]
    public async Task<ActionResult> UpdatetBook(BookUpdateDto bookUpdateDto, Guid bookId)
    {
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book");

        book.Name = bookUpdateDto.Name ?? book.Name;
        book.Year = bookUpdateDto.Year ?? book.Year;
        book.ImageUrl = bookUpdateDto.ImageUrl ?? book.ImageUrl;
        book.Price = bookUpdateDto.Price ?? book.Price;
        if (bookUpdateDto.AuthorId != null)
        {
            var author = await unitOfWork.AuthorRepository.GetAuthorByIdAsync((Guid)bookUpdateDto.AuthorId);
            if (author == null) return BadRequest("Failed to find author of given id");
            book.AuthorId = (Guid)bookUpdateDto.AuthorId;
        }
        if (bookUpdateDto.PublisherId != null)
        {
            var publisher = await unitOfWork.PublisherRepository.GetPublisherByIdAsync((Guid)bookUpdateDto.PublisherId);
            if (publisher == null) return BadRequest("Failed to find publisher of given id");
            book.PublisherId = (Guid)bookUpdateDto.PublisherId;
        }

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update book");
    }

    [HttpDelete("{bookId}")]
    public async Task<ActionResult> DeleteBook(Guid bookId)
    {  
        var book = await unitOfWork.BookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book");

        unitOfWork.BookRepository.DeleteBook(book);

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
