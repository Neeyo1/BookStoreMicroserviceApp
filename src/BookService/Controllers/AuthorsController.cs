using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

public class AuthorsController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await unitOfWork.AuthorRepository.GetAuthorsAsync();
        
        return Ok(authors);
    }

    [HttpGet("{authorId}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
    {
        var author = await unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
        if (author == null) return NotFound();

        return Ok(mapper.Map<AuthorDto>(author));
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorCreateDto authorCreateDto)
    {
        var author = mapper.Map<Author>(authorCreateDto);

        unitOfWork.AuthorRepository.AddAuthor(author);

        var newAuthor = mapper.Map<AuthorDto>(author);

        if (await unitOfWork.Complete())
            return CreatedAtAction(nameof(GetAuthor), new {authorId = author.Id}, newAuthor);
            
        return BadRequest("Failed to create author");
    }

    [HttpPut("{authorId}")]
    public async Task<ActionResult> UpdatetAuthor(AuthorCreateDto authorUpdateDto, Guid authorId)
    {
        var author = await unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
        if (author == null) return BadRequest("Failed to find author");

        author.FirstName = authorUpdateDto.FirstName ?? author.FirstName;
        author.LastName = authorUpdateDto.LastName ?? author.LastName;
        author.Alias = authorUpdateDto.Alias ?? author.Alias;

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update author");
    }

    [HttpDelete("{authorId}")]
    public async Task<ActionResult> DeleteAuthor(Guid authorId)
    {  
        var author = await unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
        if (author == null) return BadRequest("Failed to find author");

        unitOfWork.AuthorRepository.DeleteAuthor(author);

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to delete author");
    }
}
