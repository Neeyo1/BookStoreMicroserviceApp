using AutoMapper;
using Contracts;
using SearchService.DTOs;
using SearchService.Entities;

namespace SearchService.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<BookCreated, Book>();
        CreateMap<BookUpdated, Book>();
        CreateMap<(IReadOnlyList<Book> Results, long TotalCount, int PageCount), SearchDto>()
            .ConvertUsing<TupleToSearchDtoConverter>();
    }
}
