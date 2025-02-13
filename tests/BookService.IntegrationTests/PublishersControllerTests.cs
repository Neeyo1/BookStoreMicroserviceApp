using System.Net;
using System.Net.Http.Json;
using BookService.Data;
using BookService.DTOs;
using BookService.IntegrationTests.Fixtures;
using BookService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookService.IntegrationTests;

[Collection("Shared collection")]
public class PublishersControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient httpClient = factory.CreateClient();
    private const string PUBLISHER_ID = "ba8ad35f-c95a-474c-a8e6-245e11339d01";

    [Fact]
    public async Task GetPublishers_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync("api/publishers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPublishers_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync("api/publishers");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPublishers_WithValidAuth_ShouldReturn3Publishers()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<PublisherDto>>("api/publishers");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count());
    }

    [Fact]
    public async Task GetPublisherById_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync($"api/publishers/{PUBLISHER_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPublisherById_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync($"api/publishers/{PUBLISHER_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPublisherById_WithValidAuthAndInvalidId_ShouldReturnNotFound()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetAsync($"api/publishers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPublisherById_WithValidAuthAndWithValidId_ShouldReturnPublisher()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<PublisherDto>($"api/publishers/{PUBLISHER_ID}");

        Assert.NotNull(response);
        Assert.Equal("GoldPublish", response.Name);
    }

    [Fact]
    public async Task CreatePublisher_WithNoAuth_ShouldReturnUnauthorized()
    {
        var publisher = GetPublisherToCreate();

        var response = await httpClient.PostAsJsonAsync("api/publishers", publisher);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePublisher_WithIvalidAuth_ShouldReturnForbidden()
    {
        var publisher = GetPublisherToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PostAsJsonAsync("api/publishers", publisher);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreatePublisher_WithValidAuth_ShouldReturnCreated()
    {
        var publisher = GetPublisherToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PostAsJsonAsync("api/publishers", publisher);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPublisher = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(createdPublisher);
        Assert.Equal(publisher.Name, createdPublisher.Name);
    }

    [Fact]
    public async Task UpdatePublisher_WithNoAuth_ShouldReturnUnauthorized()
    {
        var publisher = GetPublisherToUpdateNewData();
        var response = await httpClient.PutAsJsonAsync($"api/publishers/{PUBLISHER_ID}", publisher);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_WithIvalidAuth_ShouldReturnForbidden()
    {
        var publisher = GetPublisherToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PutAsJsonAsync($"api/publishers/{PUBLISHER_ID}", publisher);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        var publisher = GetPublisherToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/publishers/{Guid.NewGuid()}", publisher);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_WithValidAuthAndValidIdAndSamePublisherData_ShouldReturnBadRequest()
    {
        var publisher = GetPublisherToUpdateSameData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/publishers/{PUBLISHER_ID}", publisher);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_WithValidAuthAndValidIdAndNewPublisherData_ShouldReturnNoContent()
    {
        var publisher = GetPublisherToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/publishers/{PUBLISHER_ID}", publisher);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePublisher_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.DeleteAsync($"api/publishers/{PUBLISHER_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePublisher_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.DeleteAsync($"api/publishers/{PUBLISHER_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeletePublisher_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/publishers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeletePublisher_WithValidAuthAndWithValidId_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/publishers/{PUBLISHER_ID}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        DbHelper.ReinitDbForTests(context);

        return Task.CompletedTask;
    }

    private static PublisherCreateDto GetPublisherToCreate()
    {
        return new PublisherCreateDto
        {
            Name = "PlatiniumPublish",
            Country = "Poland",
            City = "Warsaw",
            Address = "Opolska 2",
            PhoneNumber = "123123123"
        };
    }

    private static PublisherUpdateDto GetPublisherToUpdateSameData()
    {
        return new PublisherUpdateDto
        {
            Name = "GoldPublish",
        };
    }

    private static PublisherUpdateDto GetPublisherToUpdateNewData()
    {
        return new PublisherUpdateDto
        {
            Name = "DiamondPublish",
        };
    }
}
