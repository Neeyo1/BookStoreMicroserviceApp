using System.Net;
using System.Net.Http.Json;
using BookService.Data;
using BookService.DTOs;
using BookService.IntegrationTests.Fixtures;
using BookService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookService.IntegrationTests;

[Collection("Shared collection")]
public class BooksControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient httpClient = factory.CreateClient();
    private const string BOOK_ID = "d88ff401-9a0a-4660-a290-ea11ddbe5383";

    [Fact]
    public async Task GetBooks_ShouldReturn3Books()
    {
        var response = await httpClient.GetFromJsonAsync<IEnumerable<BookDto>>("api/books");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count());
    }

    [Fact]
    public async Task GetBookById_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await httpClient.GetAsync($"api/books/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBookById_WithValidId_ShouldReturnBook()
    {
        var response = await httpClient.GetFromJsonAsync<BookDto>($"api/books/{BOOK_ID}");

        Assert.NotNull(response);
        Assert.Equal("Test book 1", response.Name);
    }

    [Fact]
    public async Task CreateBook_WithNoAuth_ShouldReturnUnauthorized()
    {
        var book = GetBookToCreate();

        var response = await httpClient.PostAsJsonAsync("api/books", book);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithIvalidAuth_ShouldReturnForbidden()
    {
        var book = GetBookToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PostAsJsonAsync("api/books", book);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithValidAuthAndInvalidAuthor_ShouldReturnBadRequest()
    {
        var book = GetBookToCreate();
        book.AuthorId = Guid.NewGuid();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PostAsJsonAsync("api/books", book);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithValidAuthAndInvalidPublisher_ShouldReturnBadRequest()
    {
        var book = GetBookToCreate();
        book.PublisherId = Guid.NewGuid();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PostAsJsonAsync("api/books", book);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithValidAuthAndValidData_ShouldReturnCreated()
    {
        var book = GetBookToCreate();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PostAsJsonAsync("api/books", book);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdBook = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(createdBook);
        Assert.Equal(book.Name, createdBook.Name);
    }

    [Fact]
    public async Task UpdateBook_WithNoAuth_ShouldReturnUnauthorized()
    {
        var book = GetBookToUpdateNewData();
        var response = await httpClient.PutAsJsonAsync($"api/books/{BOOK_ID}", book);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_WithIvalidAuth_ShouldReturnForbidden()
    {
        var book = GetBookToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.PutAsJsonAsync($"api/books/{BOOK_ID}", book);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        var book = GetBookToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/books/{Guid.NewGuid()}", book);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_WithValidAuthAndValidIdAndSameBookData_ShouldReturnBadRequest()
    {
        var book = GetBookToUpdateSameData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/books/{BOOK_ID}", book);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_WithValidAuthAndValidIdAndNewBookData_ShouldReturnNoContent()
    {
        var book = GetBookToUpdateNewData();
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.PutAsJsonAsync($"api/books/{BOOK_ID}", book);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.DeleteAsync($"api/books/{BOOK_ID}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.DeleteAsync($"api/books/{BOOK_ID}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithValidAuthAndInvalidId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/books/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithValidAuthAndWithValidId_ShouldReturnNoContent()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.DeleteAsync($"api/books/{BOOK_ID}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithNoAuth_ShouldReturnUnauthorized()
    {
        var response = await httpClient.GetAsync($"api/books/{BOOK_ID}/items");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithInvalidAuth_ShouldReturnForbidden()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Moderator"));
        var response = await httpClient.GetAsync($"api/books/{BOOK_ID}/items");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithValidAuthAndInvalidBookId_ShouldReturnBadRequest()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetAsync($"api/books/{Guid.NewGuid()}/items");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetItems_WithValidAuthAndValidBookId_ShouldReturn3Items()
    {
        httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForRole("Admin"));
        var response = await httpClient.GetFromJsonAsync<IEnumerable<ItemDto>>($"api/books/{BOOK_ID}/items");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        DbHelper.ReinitDbForTests(context);

        return Task.CompletedTask;
    }

    private static BookCreateDto GetBookToCreate()
    {
        return new BookCreateDto
        {
            Name = "Minerals guide",
            Year = 2024,
            ImageUrl = "image.url",
            Price = 120,
            AuthorId = Guid.Parse("17019aeb-10c3-4fd5-92c8-83487985344b"),
            PublisherId = Guid.Parse("ba8ad35f-c95a-474c-a8e6-245e11339d01")
        };
    }

    private static BookUpdateDto GetBookToUpdateSameData()
    {
        return new BookUpdateDto
        {
            Price = 30
        };
    }

    private static BookUpdateDto GetBookToUpdateNewData()
    {
        return new BookUpdateDto
        {
            Price = 300
        };
    }
}
