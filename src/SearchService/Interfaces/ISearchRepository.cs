using MongoDB.Driver;
using SearchService.Entities;
using SearchService.Helpers;

namespace SearchService.Interfaces;

public interface ISearchRepository
{
    Task<(IReadOnlyList<Book> Results, long TotalCount, int PageCount)> GetBooks(SearchParams searchParams);
    Task<DeleteResult> DeleteBook(Guid id);
    Task<UpdateResult> UpdateBook(Book book);
}
