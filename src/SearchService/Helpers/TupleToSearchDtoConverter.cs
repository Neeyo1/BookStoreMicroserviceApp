using AutoMapper;
using SearchService.DTOs;
using SearchService.Entities;

namespace SearchService.Helpers;

public class TupleToSearchDtoConverter : ITypeConverter<(IReadOnlyList<Book> Results, long TotalCount, int PageCount), SearchDto>
{
    public SearchDto Convert((IReadOnlyList<Book> Results, long TotalCount, int PageCount) source, SearchDto destination, ResolutionContext context)
    {
        return new SearchDto
        {
            Results = source.Results,
            TotalCount = source.TotalCount,
            PageCount = source.PageCount
        };
    }
}
