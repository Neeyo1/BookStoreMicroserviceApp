using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using Contracts;

namespace BookService.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Author, AuthorDto>();
        CreateMap<AuthorCreateDto, Author>();
        CreateMap<Publisher, PublisherDto>();
        CreateMap<PublisherCreateDto, Publisher>();
        CreateMap<Book, BookDto>()
            .ForMember(x => x.Items, y => y.MapFrom(z => z.Items.Where(a => a.Status == Status.Avaiable).Count()));
        CreateMap<BookCreateDto, Book>();
        CreateMap<Item, ItemDto>();
        CreateMap<BookDto, BookCreated>();
        CreateMap<Book, BookUpdated>()
            .ForMember(x => x.Items, y => y.MapFrom(z => z.Items.Where(a => a.Status == Status.Avaiable).Count()));
        CreateMap<Item, ItemCreated>();

        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
