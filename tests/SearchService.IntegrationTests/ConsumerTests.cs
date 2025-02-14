using AutoFixture;
using Contracts;
using MassTransit.Testing;
using MongoDB.Entities;
using SearchService.Consumers;
using SearchService.Entities;
using SearchService.IntegrationTests.Fixtures;

namespace SearchService.IntegrationTests;

public class ConsumerTests(CustomWebAppFactory factory) : IClassFixture<CustomWebAppFactory>
{
    private readonly ITestHarness testHarness = factory.Services.GetTestHarness();
    private readonly Fixture fixture = new Fixture();

    [Fact]
    public async Task BookCreated_ShouldCreateBookInDb()
    {
        var consumerHarness = testHarness.GetConsumerHarness<BookCreatedConsumer>();
        var bookCreated = fixture.Create<BookCreated>();
        
        await testHarness.Bus.Publish(bookCreated);
        
        Assert.True(await consumerHarness.Consumed.Any<BookCreated>());
        var book = await DB.Find<Book>().OneAsync(bookCreated.Id.ToString());
        Assert.Equal(bookCreated.AuthorAlias, book?.AuthorAlias);
    }

    [Fact]
    public async Task BookUpdated_ShouldUpdateBookInDb()
    {
        var consumerHarness = testHarness.GetConsumerHarness<BookUpdatedConsumer>();
        var bookToCreate = fixture.Create<Book>();
        var bookUpdated = fixture.Create<BookUpdated>();
        bookToCreate.ID = Guid.NewGuid().ToString();
        bookUpdated.Id = Guid.Parse(bookToCreate.ID);
        await bookToCreate.SaveAsync();

        await testHarness.Bus.Publish(bookUpdated);
        
        Assert.True(await consumerHarness.Consumed.Any<BookUpdated>());
        var book = await DB.Find<Book>().OneAsync(bookUpdated.Id.ToString());
        Assert.Equal(bookUpdated.Price, book?.Price);
    }

    [Fact]
    public async Task BookDeleted_ShouldDeleteBookInDb()
    {
        var consumerHarness = testHarness.GetConsumerHarness<BookDeletedConsumer>();
        var bookToCreate = fixture.Create<Book>();
        var bookDeleted = fixture.Create<BookDeleted>();
        bookToCreate.ID = Guid.NewGuid().ToString();
        bookDeleted.Id = Guid.Parse(bookToCreate.ID);
        await bookToCreate.SaveAsync();
        
        await testHarness.Bus.Publish(bookDeleted);
        
        Assert.True(await consumerHarness.Consumed.Any<BookDeleted>());
        var book = await DB.Find<Book>().OneAsync(bookDeleted.Id.ToString());
        Assert.Null(book);
    }
}
