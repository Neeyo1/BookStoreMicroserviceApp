using AutoMapper;
using CartService.DTOs;
using CartService.Entities;

namespace CartService.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Book, BookDto>();
        CreateMap<BookCart, BookCartDto>();
        CreateMap<Cart, CartDto>();
    }
}
