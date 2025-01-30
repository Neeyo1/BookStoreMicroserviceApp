using AutoMapper;
using BookService.DTOs;
using BookService.Entities;

namespace BookService.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Author, AuthorDto>();
        CreateMap<AuthorCreateDto, Author>();
        CreateMap<Publisher, PublisherDto>();
        CreateMap<PublisherCreateDto, Publisher>();
        
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
