using MongoDB.Driver;
using MongoDB.Entities;
using Nest;
using SearchService.Entities;
using SearchService.Helpers;
using SearchService.Interfaces;

namespace SearchService.Data;

public class SearchRepository(IElasticClient elasticClient) : ISearchRepository
{
    public async Task<(IReadOnlyList<Book> Results, long TotalCount, int PageCount)> GetBooks(SearchParams searchParams)
    {
        var searchResponse = await elasticClient.SearchAsync<BookES>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.Name)
                        .Field(p => p.AuthorAlias))
                    .Query(searchParams.SearchTerm)))
            .From((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Size(searchParams.PageSize)
        );

        if (!searchResponse.IsValid || searchResponse.Documents.Count == 0)
        {
            return (new List<Book>(), 0, 0);
        }

        var totalCount = searchResponse.Total;
        var pageCount = (int)Math.Ceiling((double)totalCount / searchParams.PageSize);

        var bookIds = searchResponse.Documents
            .Select(x => x.Id)
            .ToList();

        var books = await DB.Find<Book>()
            .Match(x => bookIds.Contains(x.ID))
            .ExecuteAsync();

        return (books, totalCount, pageCount);
    }

    public async Task<DeleteResult> DeleteBook(Guid id)
    {
        return await DB.DeleteAsync<Book>(id.ToString());
    }

    public async Task<UpdateResult> UpdateBook(Book book)
    {
        return await DB.Update<Book>()
            .Match(x => x.ID == book.ID)
            .ModifyOnly(x => new
            {
                x.Price,
                x.Items,
                x.ImageUrl
            }, book)
            .ExecuteAsync();
    }
}
