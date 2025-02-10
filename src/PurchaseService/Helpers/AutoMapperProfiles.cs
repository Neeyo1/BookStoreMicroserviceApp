using AutoMapper;
using Contracts;
using Contracts.Cart;
using PurchaseService.Entities;

namespace PurchaseService.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CartCreated, Cart>()
            .ForMember(x => x.Items, y => y.MapFrom(z => z.Items.ToList()));
        CreateMap<BookCartContract, BookCart>()
            .ForMember(x => x.Book, y => y.MapFrom(z => z.Book));
        CreateMap<BookContract, Book>();
        CreateMap<CartUpdated, Cart>()
            .ForMember(x => x.Items, y => y.MapFrom(z => z.Items.ToList()));
        CreateMap<ItemCreated, Item>();
        CreateMap<Item, ItemUpdated>();
        CreateMap<Cart, CartStatusChanged>();
    }
}
