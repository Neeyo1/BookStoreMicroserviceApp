using SearchService.Entities;

namespace SearchService.Interfaces;

public interface ISearchRepository
{
    Task<IEnumerable<Book>> GetBooks(string? searchTerm);
}
