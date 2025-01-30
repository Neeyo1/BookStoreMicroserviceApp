using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class PublisherRepository(BookDbContext context, IMapper mapper) : IPublisherRepository
{
    public void AddPublisher(Publisher publisher)
    {
        context.Publishers.Add(publisher);
    }

    public void DeletePublisher(Publisher publisher)
    {
        context.Publishers.Remove(publisher);
    }

    public async Task<IEnumerable<PublisherDto>> GetPublishersAsync()
    {
        return await context.Publishers
            .ProjectTo<PublisherDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Publisher?> GetPublisherByIdAsync(Guid publisherId)
    {
        return await context.Publishers
            .FindAsync(publisherId);
    }
}
