using System.Net;
using System.Net.Http.Json;
using CartService.Data;
using CartService.DTOs;
using CartService.IntegrationTests.Fixtures;
using CartService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.IntegrationTests;

[Collection("Shared collection")]
public class CartsControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient httpClient = factory.CreateClient();
    private const string CART_ID = "d3f14d6a-2278-4275-998f-0b6db4905074";
    private const string BOOK_ID = "ccd7f7eb-6684-4fab-bdc3-05006b42087c";
    private const string USER_CART_ACTIVE = "bob";
    private const string USER_CART_PROCEEDING = "tob";
    private const string USER_CART_FINISHED = "tom";
    private const string USER_NO_CART = "jacob";

    [Fact]
    public async Task GetCarts_WithValidAuth_ShouldReturn3Carts()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<CartDto>>("api/carts");

        Assert.NotNull(response);
        Assert.Single(response);
    }

    [Fact]
    public async Task GetCarts_WithInvalidAuth_ShouldReturn0Carts()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_NO_CART));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<CartDto>>("api/carts");

        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task GetCarts_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync("api/carts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCartById_WithValidData_ShouldReturnCart()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.GetFromJsonAsync<CartDto>($"api/carts/{CART_ID}");

        Assert.NotNull(response);
        Assert.Equal("bob", response.Username);
    }

    [Fact]
    public async Task GetCartById_WithInvalidAuth_ShouldReturnUnauthorized()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_NO_CART));
        var response = await httpClient.GetAsync($"api/carts/{CART_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCartById_WithInvalidId_ShouldReturnNotFound()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.GetAsync($"api/carts/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCartById_WithNoAuth_ShouldReturnUnuthorized()
    {
        var response = await httpClient.GetAsync($"api/carts/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithValidDataAndExistingCart_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithValidDataAndNonExistingCart_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_NO_CART));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithCartProceeding_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_PROCEEDING));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithCartFinished_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_FINISHED));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithInvalidBookId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={Guid.NewGuid()}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithInvalidQuantity_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=-5", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.PutAsync(
            $"api/carts/add?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithValidDataAndExistingCart_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithValidDataAndNonExistingCart_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_NO_CART));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithCartProceeding_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_PROCEEDING));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithCartFinished_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_FINISHED));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithInvalidBookId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={Guid.NewGuid()}&quantity=1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithInvalidQuantity_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_CART_ACTIVE));
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=-5", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromCart_WithNoIdentity_ShouldReturnUnauthorized()
    {
        var response = await httpClient.PutAsync(
            $"api/carts/remove?bookId={BOOK_ID}&quantity=1", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
        DbHelper.ReinitDbForTests(context);

        return Task.CompletedTask;
    }
}
