using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
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
}
