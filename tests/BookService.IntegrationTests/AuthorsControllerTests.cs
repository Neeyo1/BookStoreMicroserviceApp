using System.Net;
using System.Net.Http.Json;
using BookService.Data;
using BookService.DTOs;
using BookService.IntegrationTests.Fixtures;
using BookService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookService.IntegrationTests;

[Collection("Shared collection")]
public class AuthorsControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient httpClient = factory.CreateClient();
    private const string AUTHOR_ID = "17019aeb-10c3-4fd5-92c8-83487985344b";

    [Fact]
    public async Task GetAuthors_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync("api/authors");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync("api/authors");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_WithValidAuth_ShouldReturn3Authors()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<AuthorDto>>("api/authors");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count());
    }

    [Fact]
    public async Task GetAuthorById_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync($"api/authors/{AUTHOR_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthorById_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync($"api/authors/{AUTHOR_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthorById_WithValidAuthAndInvalidId_ShouldReturnNotFound()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetAsync($"api/authors/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthorById_WithValidAuthAndWithValidId_ShouldReturnAuthor()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<AuthorDto>($"api/authors/{AUTHOR_ID}");

        Assert.NotNull(response);
        Assert.Equal("Jack", response.FirstName);
    }

    [Fact]
    public async Task CreateAuthor_WithNoAuth_ShouldReturnUnauthorized()
    {
        var author = GetAuthorToCreate();

        var response = await httpClient.PostAsJsonAsync("api/authors", author);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuthor_WithIvalidAuth_ShouldReturnForbidden()
    {
        var author = GetAuthorToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PostAsJsonAsync("api/authors", author);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuthor_WithValidAuth_ShouldReturnCreated()
    {
        var author = GetAuthorToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PostAsJsonAsync("api/authors", author);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuthor = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(createdAuthor);
        Assert.Equal(author.FirstName, createdAuthor.FirstName);
    }

    [Fact]
    public async Task UpdateAuthor_WithNoAuth_ShouldReturnUnauthorized()
    {
        var author = GetAuthorToUpdateNewData();
        var response = await httpClient.PutAsJsonAsync($"api/authors/{AUTHOR_ID}", author);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_WithIvalidAuth_ShouldReturnForbidden()
    {
        var author = GetAuthorToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PutAsJsonAsync($"api/authors/{AUTHOR_ID}", author);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        var author = GetAuthorToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/authors/{Guid.NewGuid()}", author);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_WithValidAuthAndValidIdAndSameAuthorData_ShouldReturnBadRequest()
    {
        var author = GetAuthorToUpdateSameData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/authors/{AUTHOR_ID}", author);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_WithValidAuthAndValidIdAndNewAuthorData_ShouldReturnNoContent()
    {
        var author = GetAuthorToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/authors/{AUTHOR_ID}", author);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.DeleteAsync($"api/authors/{AUTHOR_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.DeleteAsync($"api/authors/{AUTHOR_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/authors/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_WithValidAuthAndWithValidId_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/authors/{AUTHOR_ID}");

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

    private static AuthorCreateDto GetAuthorToCreate()
    {
        return new AuthorCreateDto
        {
            FirstName = "Monica",
            LastName = "Prem",
            Alias = "Monrem",
        };
    }

    private static AuthorCreateDto GetAuthorToUpdateSameData()
    {
        return new AuthorCreateDto
        {
            FirstName = "Jack",
        };
    }

    private static AuthorCreateDto GetAuthorToUpdateNewData()
    {
        return new AuthorCreateDto
        {
            FirstName = "John",
        };
    }
}
