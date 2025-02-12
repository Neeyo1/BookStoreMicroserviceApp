using AutoFixture;
using AutoMapper;
using BookService.Controllers;
using BookService.DTOs;
using BookService.Entities;
using BookService.Helpers;
using BookService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookService.UnitTests;

public class AuthorsControllerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly IMapper mapper;
    private readonly AuthorsController authorsController;

    public AuthorsControllerTests()
    {
        fixture = new Fixture();
        unitOfWork = new Mock<IUnitOfWork>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(AutoMapperProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        mapper = new Mapper(mockMapper);
        authorsController = new AuthorsController(unitOfWork.Object, mapper)
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
    public async Task GetAuthors_WithNoParams_Returns10Authors()
    {
        var authors = fixture.CreateMany<AuthorDto>(10).ToList();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorsAsync()).ReturnsAsync(authors);

        var result = await authorsController.GetAuthors();
        var actionResult = result.Result as OkObjectResult;
        var values = actionResult!.Value as IEnumerable<AuthorDto>;

        Assert.Equal(10, values!.Count());
        Assert.IsType<ActionResult<IEnumerable<AuthorDto>>>(result);
    }

    [Fact]
    public async Task GetAuthor_WithValidId_ReturnsAuthor()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);

        var result = await authorsController.GetAuthor(author.Id);
        var actionResult = result.Result as OkObjectResult;
        var value = actionResult!.Value as AuthorDto;

        Assert.Equal(author.Alias, value!.Alias);
        Assert.IsType<ActionResult<AuthorDto>>(result);
    }

    [Fact]
    public async Task GetAuthor_WithInvalidId_ReturnsNotFound()
    {
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await authorsController.GetAuthor(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuthor_WithValidAuthorCreateDto_ReturnsCreatedAtAction()
    {
        var author = fixture.Create<AuthorCreateDto>();
        unitOfWork.Setup(x => x.AuthorRepository.AddAuthor(It.IsAny<Author>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await authorsController.CreateAuthor(author);
        var actionResult = result.Result as CreatedAtActionResult;

        Assert.NotNull(actionResult);
        Assert.Equal("GetAuthor", actionResult.ActionName);
        Assert.IsType<AuthorDto>(actionResult.Value);
    }

    [Fact]
    public async Task CreateAuthor_FailedSave_ReturnsBadRequest()
    {
        var author = fixture.Create<AuthorCreateDto>();
        unitOfWork.Setup(x => x.AuthorRepository.AddAuthor(It.IsAny<Author>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await authorsController.CreateAuthor(author);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuthor_WithUpdateAuthorDto_ReturnsNoContent()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var authorToUpdate = fixture.Create<AuthorCreateDto>();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await authorsController.UpdatetAuthor(authorToUpdate, author.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAuthor_WithInvalidId_ReturnsBadRequest()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var authorToUpdate = fixture.Create<AuthorCreateDto>();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await authorsController.UpdatetAuthor(authorToUpdate, author.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAuthor_FailedSave_ReturnsBadRequest()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        var authorToUpdate = fixture.Create<AuthorCreateDto>();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await authorsController.UpdatetAuthor(authorToUpdate, author.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAuthor_WithValidId_ReturnsNoContent()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await authorsController.DeleteAuthor(author.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAuthor_WithInvalidId_ReturnsBadRequest()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await authorsController.DeleteAuthor(author.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAuthor_FailedSave_ReturnsBadRequest()
    {
        var author = fixture.Build<Author>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.AuthorRepository.GetAuthorByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(author);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await authorsController.DeleteAuthor(author.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
