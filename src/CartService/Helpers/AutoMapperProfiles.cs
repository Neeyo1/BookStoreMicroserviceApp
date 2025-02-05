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
        CreateMap<Cart, CartDto>()
            .ForMember(x => x.Items, y => y.MapFrom(z => z.BookCarts))
            .ForMember(x => x.TotalPrice, y => y.MapFrom(z => z.BookCarts.Sum(s => s.Book.Price * s.Quantity)));
    }
}
