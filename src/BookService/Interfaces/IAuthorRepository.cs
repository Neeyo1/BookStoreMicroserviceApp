using BookService.DTOs;
using BookService.Entities;

namespace BookService.Interfaces;

public interface IAuthorRepository
{
    void AddAuthor(Author author);
    void DeleteAuthor(Author author);
    Task<IEnumerable<AuthorDto>> GetAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(Guid authorId);
}
