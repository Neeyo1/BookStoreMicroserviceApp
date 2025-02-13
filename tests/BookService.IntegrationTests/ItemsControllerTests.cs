using System.Net;
using System.Net.Http.Json;
using BookService.Data;
using BookService.DTOs;
using BookService.IntegrationTests.Fixtures;
using BookService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookService.IntegrationTests;

[Collection("Shared collection")]
public class ItemsControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient httpClient = factory.CreateClient();
    private const string ITEM_ID = "88dcebc6-5665-4b8c-8108-387cc2437e14";
    private const string BOOK_ID = "d88ff401-9a0a-4660-a290-ea11ddbe5383";

    [Fact]
    public async Task GetItems_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync($"api/items?bookId={BOOK_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync($"api/items?bookId={BOOK_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithValidAuthAndIvnalidBookId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetAsync($"api/items?bookId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithValidAuthAndValidBookId_ShouldReturn3Items()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<ItemDto>>($"api/items?bookId={BOOK_ID}");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count());
    }

    [Fact]
    public async Task GetItemById_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync($"api/items/{ITEM_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetItemById_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync($"api/items/{ITEM_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetItemById_WithValidAuthAndInvalidId_ShouldReturnNotFound()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetAsync($"api/items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetItemById_WithValidAuthAndWithValidId_ShouldReturnItem()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<ItemDto>($"api/items/{ITEM_ID}");

        Assert.NotNull(response);
        Assert.Equal(Guid.Parse(ITEM_ID), response.Id);
    }

    [Fact]
    public async Task CreateItems_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.PutAsync($"api/items?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateItems_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PutAsync($"api/items?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateItems_WithValidAuthAndInvalidBookId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsync($"api/items?bookId={Guid.NewGuid()}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithValidAuthAndInvalidQuantity_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsync($"api/items?bookId={BOOK_ID}&quantity=-1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithValidAuthAndValidData_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsync($"api/items?bookId={BOOK_ID}&quantity=1", null);

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
}
