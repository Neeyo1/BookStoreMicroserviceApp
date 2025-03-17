using System.Text.Json;
using AutoMapper;
using MongoDB.Driver;
using MongoDB.Entities;
using Nest;
using SearchService.Entities;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(string connectionString, ILogger<DbInitializer> logger)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(connectionString));
        
        await DB.Index<Book>()
            .Key(x => x.Name, KeyType.Text)
            .Key(x => x.AuthorFirstName, KeyType.Text)
            .Key(x => x.AuthorLastName, KeyType.Text)
            .Key(x => x.AuthorAlias, KeyType.Text)
            .Key(x => x.PublisherName, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Book>();
        logger.LogInformation("------ Seeding process started ------");
        if (count == 0)
        {
            var bookData = await File.ReadAllTextAsync("Data/books.json");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var books = JsonSerializer.Deserialize<IEnumerable<Book>>(bookData, options);
            if (books == null)
                return;
                
            await DB.SaveAsync(books);
            logger.LogInformation("------ Seeding process completed ------");
        }
        else
        {
            logger.LogInformation("------ Seeding process skipped ------");
        }
    }

    public static async Task IndexDocuments(IElasticClient elasticClient, IMapper mapper,
        ILogger<DbInitializer> logger)
    {
        var books = await DB.Find<Book>()
            .ExecuteAsync();

        var indicesResponse = await elasticClient.Cat.IndicesAsync();
        if (indicesResponse.IsValid)
        {
            var indexCount = indicesResponse.Records.Count;
            if (indexCount == books.Count)
            {
                logger.LogInformation("------ All books already indexed ------");
                return;
            }
        }
        else
        {
            logger.LogError("------ Failed to get index count ------");
            return;
        }

        var successCount = 0;

        foreach (var book in books)
        {
            var indexResponse = await elasticClient.IndexDocumentAsync(mapper.Map<BookES>(book));

            if (indexResponse.IsValid)
            {
                successCount++;
            }
        }

        logger.LogInformation("------ Successfully indexed {successCount}/{totalCount} ------",
            successCount, books.Count);
    }
}
