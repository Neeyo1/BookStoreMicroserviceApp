using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Interfaces;

namespace SearchService.Data;

public class SearchRepository : ISearchRepository
{
    public async Task<IEnumerable<Book>> GetBooks(string? searchTerm)
    {
        var query = DB.Find<Book>();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query.Match(Search.Full, searchTerm).SortByTextScore();
        }
        return await query.ExecuteAsync();
    }
}
