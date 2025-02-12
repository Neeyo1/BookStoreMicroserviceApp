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

public class BooksControllerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly IMapper mapper;
    private readonly BooksController booksController;
    private readonly Mock<IPublishEndpoint> publishEndpoint;

    public BooksControllerTests()
    {
        fixture = new Fixture();
        unitOfWork = new Mock<IUnitOfWork>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(AutoMapperProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        mapper = new Mapper(mockMapper);

        publishEndpoint = new Mock<IPublishEndpoint>();
        
        booksController = new BooksController(unitOfWork.Object, mapper, publishEndpoint.Object)
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
    public async Task GetBooks_WithNoParams_Returns10Books()
    {
        var books = fixture.CreateMany<BookDto>(10).ToList();
        unitOfWork.Setup(x => x.BookRepository.GetBooksAsync()).ReturnsAsync(books);

        var result = await booksController.GetBooks();
        var actionResult = result.Result as OkObjectResult;
        var values = actionResult!.Value as IEnumerable<BookDto>;

        Assert.Equal(10, values!.Count());
        Assert.IsType<ActionResult<IEnumerable<BookDto>>>(result);
    }

    [Fact]
    public async Task GetBook_WithValidId_ReturnsBook()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookWithDetailsByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        var result = await booksController.GetBook(book.Id);
        var actionResult = result.Result as OkObjectResult;
        var value = actionResult!.Value as BookDto;

        Assert.Equal(book.Name, value!.Name);
        Assert.IsType<ActionResult<BookDto>>(result);
    }

    [Fact]
    public async Task GetBook_WithInvalidId_ReturnsNotFound()
    {
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await booksController.GetBook(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_WithValidBookCreateDto_ReturnsCreatedAtAction()
    {
        var book = fixture.Create<BookCreateDto>();
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.BookRepository.AddBook(It.IsAny<Book>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await booksController.CreateBook(book);
        var actionResult = result.Result as CreatedAtActionResult;

        Assert.NotNull(actionResult);
        Assert.Equal("GetBook", actionResult.ActionName);
        Assert.IsType<BookDto>(actionResult.Value);
    }

    [Fact]
    public async Task CreateBook_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Create<BookCreateDto>();
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.BookRepository.AddBook(It.IsAny<Book>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await booksController.CreateBook(book);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_WithInvalidAuthor_ReturnsBadRequest()
    {
        var book = fixture.Create<BookCreateDto>();
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await booksController.CreateBook(book);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_WithInvaludPublisher_ReturnsBadRequest()
    {
        var book = fixture.Create<BookCreateDto>();
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await booksController.CreateBook(book);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateBook_WithValidBookUpdateDto_ReturnsNoContent()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        var bookToUpdate = fixture.Create<BookUpdateDto>();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await booksController.UpdatetBook(bookToUpdate, book.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateBook_WithInvalidId_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        var bookToUpdate = fixture.Create<BookUpdateDto>();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await booksController.UpdatetBook(bookToUpdate, book.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateBook_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        var bookToUpdate = fixture.Create<BookUpdateDto>();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await booksController.UpdatetBook(bookToUpdate, book.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteBook_WithValidId_ReturnsNoContent()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await booksController.DeleteBook(book.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBook_WithInvalidId_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await booksController.DeleteBook(book.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteBook_FailedSave_ReturnsBadRequest()
    {
        var book = fixture.Build<Book>()
            .Without(x => x.Author)
            .Without(x => x.Publisher)
            .Without(x => x.Items)
            .Create();
        unitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await booksController.DeleteBook(book.Id);

        Assert.IsType<BadRequestObjectResult>(result);
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

        var result = await booksController.GetItems(book.Id);
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

        var result = await booksController.GetItems(It.IsAny<Guid>());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
