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

public class PublishersControllerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly IMapper mapper;
    private readonly PublishersController publishersController;

    public PublishersControllerTests()
    {
        fixture = new Fixture();
        unitOfWork = new Mock<IUnitOfWork>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(AutoMapperProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        mapper = new Mapper(mockMapper);
        publishersController = new PublishersController(unitOfWork.Object, mapper)
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
    public async Task GetPublishers_WithNoParams_Returns10Publishers()
    {
        var publishers = fixture.CreateMany<PublisherDto>(10).ToList();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublishersAsync()).ReturnsAsync(publishers);

        var result = await publishersController.GetPublishers();
        var actionResult = result.Result as OkObjectResult;
        var values = actionResult!.Value as IEnumerable<PublisherDto>;

        Assert.Equal(10, values!.Count());
        Assert.IsType<ActionResult<IEnumerable<PublisherDto>>>(result);
    }

    [Fact]
    public async Task GetPublisher_WithValidId_ReturnsPublisher()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);

        var result = await publishersController.GetPublisher(publisher.Id);
        var actionResult = result.Result as OkObjectResult;
        var value = actionResult!.Value as PublisherDto;

        Assert.Equal(publisher.Name, value!.Name);
        Assert.IsType<ActionResult<PublisherDto>>(result);
    }

    [Fact]
    public async Task GetPublisher_WithInvalidId_ReturnsNotFound()
    {
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await publishersController.GetPublisher(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreatePublisher_WithValidPublisherCreateDto_ReturnsCreatedAtAction()
    {
        var publisher = fixture.Create<PublisherCreateDto>();
        unitOfWork.Setup(x => x.PublisherRepository.AddPublisher(It.IsAny<Publisher>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await publishersController.CreatePublisher(publisher);
        var actionResult = result.Result as CreatedAtActionResult;

        Assert.NotNull(actionResult);
        Assert.Equal("GetPublisher", actionResult.ActionName);
        Assert.IsType<PublisherDto>(actionResult.Value);
    }

    [Fact]
    public async Task CreatePublisher_FailedSave_ReturnsBadRequest()
    {
        var publisher = fixture.Create<PublisherCreateDto>();
        unitOfWork.Setup(x => x.PublisherRepository.AddPublisher(It.IsAny<Publisher>()));
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await publishersController.CreatePublisher(publisher);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePublisher_WithValidPublisherUpdateDto_ReturnsNoContent()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        var publisherToUpdate = fixture.Create<PublisherUpdateDto>();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await publishersController.UpdatetPublisher(publisherToUpdate, publisher.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdatePublisher_WithInvalidId_ReturnsBadRequest()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        var publisherToUpdate = fixture.Create<PublisherUpdateDto>();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await publishersController.UpdatetPublisher(publisherToUpdate, publisher.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdatePublisher_FailedSave_ReturnsBadRequest()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        var publisherToUpdate = fixture.Create<PublisherUpdateDto>();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await publishersController.UpdatetPublisher(publisherToUpdate, publisher.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeletePublisher_WithValidId_ReturnsNoContent()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(true);

        var result = await publishersController.DeletePublisher(publisher.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePublisher_WithInvalidId_ReturnsBadRequest()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        var result = await publishersController.DeletePublisher(publisher.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeletePublisher_FailedSave_ReturnsBadRequest()
    {
        var publisher = fixture.Build<Publisher>().Without(x => x.Books).Create();
        unitOfWork.Setup(x => x.PublisherRepository.GetPublisherByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(publisher);
        unitOfWork.Setup(x => x.Complete()).ReturnsAsync(false);

        var result = await publishersController.DeletePublisher(publisher.Id);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
