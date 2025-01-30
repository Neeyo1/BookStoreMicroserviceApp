using BookService.DTOs;
using BookService.Entities;

namespace BookService.Interfaces;

public interface IPublisherRepository
{
    void AddPublisher(Publisher publisher);
    void DeletePublisher(Publisher publisher);
    Task<IEnumerable<PublisherDto>> GetPublishersAsync();
    Task<Publisher?> GetPublisherByIdAsync(Guid publisherId);
}
