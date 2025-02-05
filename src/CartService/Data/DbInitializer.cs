using System.Text.Json;
using CartService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data;

public class DbInitializer
{
    public static async Task InitDb(CartDbContext context, ILogger<DbInitializer> logger)
    {
        await SeedData(context, logger);
    }
    private static async Task SeedData(CartDbContext context, ILogger<DbInitializer> logger)
    {
        logger.LogInformation("------ Seeding process started ------");

        if (await context.Books.AnyAsync())
        {
            logger.LogInformation("------ Seeding process skipped ------");
            return;
        }

        var bookData = await File.ReadAllTextAsync("Data/books.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var books = JsonSerializer.Deserialize<IEnumerable<Book>>(bookData, options);
        if (books == null)
        {
            logger.LogInformation("------ Seeding process failed ------");
            return;
        }
        
        await context.Books.AddRangeAsync(books);

        await context.SaveChangesAsync();
        logger.LogInformation("------ Seeding process completed ------");
    }
}
