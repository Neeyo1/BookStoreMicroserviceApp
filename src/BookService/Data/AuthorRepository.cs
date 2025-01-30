using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class AuthorRepository(BookDbContext context, IMapper mapper) : IAuthorRepository
{
    public void AddAuthor(Author author)
    {
        context.Authors.Add(author);
    }

    public void DeleteAuthor(Author author)
    {
        context.Authors.Remove(author);
    }

    public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync()
    {
        return await context.Authors
            .ProjectTo<AuthorDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(Guid authorId)
    {
        return await context.Authors
            .FindAsync(authorId);
    }
}
