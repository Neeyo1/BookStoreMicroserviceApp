using AutoFixture;
using AutoMapper;
using CartService.Controllers;
using CartService.DTOs;
using CartService.Entities;
using CartService.Helpers;
using CartService.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartService.UnitTests;

public class CartsControllerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IBookRepository> bookRepository;
    private readonly Mock<ICartRepository> cartRepository;
    private readonly IMapper mapper;
    private readonly CartsController cartsController;
    private readonly Mock<IPublishEndpoint> publishEndpoint;

    public CartsControllerTests()
    {
        fixture = new Fixture();
        bookRepository = new Mock<IBookRepository>();
        cartRepository = new Mock<ICartRepository>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(AutoMapperProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;
        mapper = new Mapper(mockMapper);

        publishEndpoint = new Mock<IPublishEndpoint>();

        cartsController = new CartsController(cartRepository.Object, bookRepository.Object, mapper,
            publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = Utils.Helpers.GetClaimsPrincipal()
                }
            }
        };
    }

    [Fact]
    public async Task GetCarts_WithNoParams_Returns10Carts()
    {
        var carts = fixture.CreateMany<CartDto>(10).ToList();
        cartRepository.Setup(x => x.GetCartsAsync(It.IsAny<string>())).ReturnsAsync(carts);

        var result = await cartsController.GetCarts();
        var actionResult = result.Result as OkObjectResult;
        var values = actionResult!.Value as IEnumerable<CartDto>;

        Assert.Equal(10, values!.Count());
        Assert.IsType<ActionResult<IEnumerable<CartDto>>>(result);
    }

    [Fact]
    public async Task GetCart_WithValidId_ReturnsCart()
    {
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        cart.Username = "test";
        cartRepository.Setup(x => x.GetCartWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

        var result = await cartsController.GetCart(cart.Id);
        var actionResult = result.Result as OkObjectResult;
        var value = actionResult!.Value as CartDto;

        Assert.Equal(cart.CreatedAt, value!.CreatedAt);
        Assert.IsType<ActionResult<CartDto>>(result);
    }

    [Fact]
    public async Task GetCart_WithInvalidId_ReturnsNotFound()
    {
        cartRepository.Setup(x => x.GetCartByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await cartsController.GetCart(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCart_WithValidIdAndInvalidAuth_ReturnsUnauthorized()
    {
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        cartRepository.Setup(x => x.GetCartWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

        var result = await cartsController.GetCart(Guid.NewGuid());

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task AddToCart_WithExistingCart_ReturnsNoContent()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(cart);
        cartRepository.Setup(x => x.AddCart(It.IsAny<Cart>()));
        cartRepository.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await cartsController.AddToCart(Guid.NewGuid(), 1);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task AddToCart_WithNonexistingCart_ReturnsNoContent()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);
        cartRepository.Setup(x => x.AddCart(It.IsAny<Cart>()));
        cartRepository.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await cartsController.AddToCart(Guid.NewGuid(), 1);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task AddToCart_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);
        cartRepository.Setup(x => x.AddCart(It.IsAny<Cart>()));
        cartRepository.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await cartsController.AddToCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddToCart_WithInvalidBookId_ReturnsBadRequest()
    {
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await cartsController.AddToCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RemoveFromCart_WithExistingCart_ReturnsNoContent()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        var bookCarts = fixture.Build<BookCart>()
            .Without(x => x.Cart)
            .Without(x => x.Book)
            .CreateMany().ToList();
        bookCarts.ForEach(x => x.Book = book);
        cart.BookCarts = bookCarts;
        book.BookCarts = bookCarts;
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(cart);
        cartRepository.Setup(x => x.GetBookCartByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(cart.BookCarts.First());    
        cartRepository.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await cartsController.RemoveFromCart(Guid.NewGuid(), 1);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task RemoveFromCart_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        var bookCarts = fixture.Build<BookCart>()
            .Without(x => x.Cart)
            .Without(x => x.Book)
            .CreateMany().ToList();
        bookCarts.ForEach(x => x.Book = book);
        cart.BookCarts = bookCarts;
        book.BookCarts = bookCarts;
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(cart);
        cartRepository.Setup(x => x.GetBookCartByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(cart.BookCarts.First());    
        cartRepository.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await cartsController.RemoveFromCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RemoveFromCart_WithNonexistingBookCart_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        var cart = fixture.Build<Cart>().Without(x => x.BookCarts).Create();
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(cart);
        cartRepository.Setup(x => x.GetBookCartByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(value: null);    

        var result = await cartsController.RemoveFromCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RemoveFromCart_WithInvalidCart_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>().Without(x => x.BookCarts).Create();
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        cartRepository.Setup(x => x.GetActiveOrProceedingCartByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);   

        var result = await cartsController.RemoveFromCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RemoveFromCart_WithInvalidBookId_ReturnsBadRequest()
    {
        bookRepository.Setup(x => x.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null); 

        var result = await cartsController.RemoveFromCart(Guid.NewGuid(), 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
