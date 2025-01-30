using BookService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookService.Data;

public class DbInitializer
{
    public static async Task InitDb(BookDbContext context, ILogger<DbInitializer> logger)
    {
        await SeedData(context, logger);
    }
    private static async Task SeedData(BookDbContext context, ILogger<DbInitializer> logger)
    {
        logger.LogInformation("------ Seeding process started ------");
        if (await context.Authors.AnyAsync())
        {
            logger.LogInformation("------ Seeding process skipped ------");
            return;
        }
        var authors = new List<Author>
        {
            // Adam Thompson aka Adamth
            new() {
                Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                FirstName = "Adam",
                LastName = "Thompson",
                Alias = "Adamth"
            },
            // Juliet Carl aka J.L.T
            new() {
                Id = Guid.Parse("ac9adc10-5cb5-49be-a74a-ac4179a573ce"),
                FirstName = "Juliet",
                LastName = "Carl",
                Alias = "J.L.T"
            },
            // Tom Tomas aka TomTom
            new() {
                Id = Guid.Parse("51448aec-e3c8-403e-bfeb-b8c906511469"),
                FirstName = "Tom",
                LastName = "Tomas",
                Alias = "TomTom"
            }
        };
        await context.Authors.AddRangeAsync(authors);

        if (await context.Publishers.AnyAsync())
        {
            logger.LogInformation("------ Seeding process skipped ------");
            return;
        }
        var publishers = new List<Publisher>
        {
            // DreamPublish
            new() {
                Id = Guid.Parse("7edb654f-d6c7-467e-b2ce-82662763f204"),
                Name = "DreamPublish",
                Country = "Poland",
                City = "Warsaw",
                Address = "Miodowa 1",
                PhoneNumber = "123456789"
            },
            // BokkStore
            new() {
                Id = Guid.Parse("73dfe47b-b447-42f0-babc-bcf63d63c8d6"),
                Name = "BokkStore",
                Country = "Poland",
                City = "Warsaw",
                Address = "Wolna 1",
                PhoneNumber = "987654321"
            },
            // Books&Novels
            new() {
                Id = Guid.Parse("6029e7fd-47f6-4583-b3db-239a6fc446a5"),
                Name = "Books&Novels",
                Country = "Poland",
                City = "Warsaw",
                Address = "Okulickiego 1",
                PhoneNumber = "123123123"
            },
        };
        await context.Publishers.AddRangeAsync(publishers);

        if (await context.Books.AnyAsync())
        {
            logger.LogInformation("------ Seeding process skipped ------");
            return;
        }
        var books = new List<Book>
        {
            // Magic beasts
            new() {
                Id = Guid.Parse("ea892d58-b359-4f65-9c2b-814137ff00b3"),
                Name = "Magic beasts",
                Year = 2022,
                Price = 30,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                PublisherId = Guid.Parse("7edb654f-d6c7-467e-b2ce-82662763f204")
            },
            // Facts about cosmos
            new() {
                Id = Guid.Parse("f7d82044-3288-484f-8d6e-3edbcda097b4"),
                Name = "Facts about cosmos",
                Year = 2024,
                Price = 51,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("ac9adc10-5cb5-49be-a74a-ac4179a573ce"),
                PublisherId = Guid.Parse("73dfe47b-b447-42f0-babc-bcf63d63c8d6")
            },
            // Wild life
            new() {
                Id = Guid.Parse("fba945a2-bcd9-48b0-8c6a-0bde476c00cc"),
                Name = "Wild life",
                Year = 2019,
                Price = 15,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("51448aec-e3c8-403e-bfeb-b8c906511469"),
                PublisherId = Guid.Parse("6029e7fd-47f6-4583-b3db-239a6fc446a5")
            }
        };
        await context.Books.AddRangeAsync(books);

        if (await context.Items.AnyAsync())
        {
            logger.LogInformation("------ Seeding process skipped ------");
            return;
        }
        var items = new List<Item>
        {
            // Magic beasts x2
            new() {
                Id = Guid.Parse("441256d3-83a1-4419-8693-67aca2435ba1"),
                BookId = Guid.Parse("ea892d58-b359-4f65-9c2b-814137ff00b3")
            },
            new() {
                Id = Guid.Parse("b2b8a7d2-5845-45af-adf3-d2cf4a3c2aa1"),
                BookId = Guid.Parse("ea892d58-b359-4f65-9c2b-814137ff00b3")
            },
            // Facts about cosmos x2
            new() {
                Id = Guid.Parse("266bc958-0f05-4874-a861-fe2d754ea6b0"),
                BookId = Guid.Parse("f7d82044-3288-484f-8d6e-3edbcda097b4")
            },
            new() {
                Id = Guid.Parse("3264e4df-6c71-4522-9d9a-a958b7412874"),
                BookId = Guid.Parse("f7d82044-3288-484f-8d6e-3edbcda097b4")
            },
            // Wild life
            new() {
                Id = Guid.Parse("82bb6e14-8980-4fcf-a8ce-8946208f90e2"),
                BookId = Guid.Parse("fba945a2-bcd9-48b0-8c6a-0bde476c00cc")
            }
        };
        await context.Books.AddRangeAsync(books);

        await context.SaveChangesAsync();
        logger.LogInformation("------ Seeding process completed ------");
    }
}
