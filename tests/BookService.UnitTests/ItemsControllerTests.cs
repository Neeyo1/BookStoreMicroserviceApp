using AutoFixture;
using AutoMapper;
using BookService.Controllers;
using BookService.DTOs;
using BookService.Entities;
using BookService.Helpers;
using BookService.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookService.UnitTests;

public class ItemsControllerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly IMapper mapper;
    private readonly ItemsController itemsController;
    private readonly Mock<IPublishEndpoint> publishEndpoint;

    public ItemsControllerTests()
    {
        fixture = new Fixture();
        unitOfWork = new Mock<IUnitOfWork>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(AutoMapperProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        mapper = new Mapper(mockMapper);

        publishEndpoint = new Mock<IPublishEndpoint>();
        
        itemsController = new ItemsController(unitOfWork.Object, mapper, publishEndpoint.Object)
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
    public async Task GetItems_WithValidBook_Returns10Items()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        var items = fixture.CreateMany<ItemDto>(10).ToList();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.ItemRepository.GetItemsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(items);

        var result = await itemsController.GetItems(book.Id);
        var actionResult = result.Result as OkObjectResult;
        var values = actionResult!.Value as IEnumerable<ItemDto>;

        Assert.Equal(10, values!.Count());
        Assert.IsType<ActionResult<IEnumerable<ItemDto>>>(result);
    }

    [Fact]
    public async Task GetItems_WithInvalidBook_ReturnsBadRequest()
    {
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await itemsController.GetItems(It.IsAny<Guid>());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetItem_WithValidId_ReturnsItem()
    {
        var item = fixture.Build<Item>().Without(x => x.Book).Create();
        unitOfWork.Setup(x => x.ItemRepository.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(item);

        var result = await itemsController.GetItem(item.Id);
        var actionResult = result.Result as OkObjectResult;
        var value = actionResult!.Value as ItemDto;

        Assert.Equal(item.CreatedAt, value!.CreatedAt);
        Assert.IsType<ActionResult<ItemDto>>(result);
    }

    [Fact]
    public async Task GetItem_WithInvalidId_ReturnsNotFound()
    {
        unitOfWork.Setup(x => x.ItemRepository.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await itemsController.GetItem(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateItems_WithValidBook_ReturnsNoContent()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.ItemRepository.AddItem(It.IsAny<Item>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await itemsController.CreateItems(book.Id, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateItems_WithInvalidBook_ReturnsBadRequest()
    {
        unitOfWork.Setup(x => x.BookRepository.GetBookWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await itemsController.CreateItems(It.IsAny<Guid>(), 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateItems_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.ItemRepository.AddItem(It.IsAny<Item>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await itemsController.CreateItems(book.Id, 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
