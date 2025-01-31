using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Helpers;
using SearchService.Interfaces;

namespace SearchService.Data;

public class SearchRepository : ISearchRepository
{
    public async Task<(IReadOnlyList<Book> Results, long TotalCount, int PageCount)> GetBooks(SearchParams searchParams)
    {
        var query = DB.PagedSearch<Book, Book>();

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }
        query = searchParams.OrderBy switch
        {
            "name" => query.Sort(x => x.Ascending(y => y.Name)),
            "name-desc" => query.Sort(x => x.Descending(y => y.Name)),
            "count" => query.Sort(x => x.Ascending(y => y.Items)),
            "count-desc" => query.Sort(x => x.Descending(y => y.Items)),
            _ => query.Sort(x => x.Ascending(y => y.Name)) // "name"
        };
        query = searchParams.FilterBy switch
        {
            "avaiable" => query.Match(x => x.Items > 0),
            "non-avaiable" => query.Match(x => x.Items == 0),
            "all" => query,
            _ => query // "all"
        };
        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        return await query.ExecuteAsync();
    }
}
