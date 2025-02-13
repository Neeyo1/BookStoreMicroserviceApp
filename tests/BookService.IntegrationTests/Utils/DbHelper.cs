using BookService.Data;
using BookService.Entities;

namespace BookService.IntegrationTests.Utils;

public static class DbHelper
{
    public static void InitDbForTests(BookDbContext context)
    {
        context.Authors.AddRange(GetAuthorsForTest());
        context.Publishers.AddRange(GetPublishersForTest());
        context.Books.AddRange(GetBooksForTest());
        context.Items.AddRange(GetItemsForTest());
        context.SaveChanges();
    }

    public static void ReinitDbForTests(BookDbContext context)
    {
        context.Authors.RemoveRange(context.Authors);
        context.Publishers.RemoveRange(context.Publishers);
        context.Books.RemoveRange(context.Books);
        context.Items.RemoveRange(context.Items);
        context.SaveChanges();
        InitDbForTests(context);
    }

    private static List<Author> GetAuthorsForTest()
    {
        return new List<Author>
        {
            // Jack Torr aka Jor
            new() {
                Id = Guid.Parse("17019aeb-10c3-4fd5-92c8-83487985344b"),
                FirstName = "Jack",
                LastName = "Torr",
                Alias = "Jor"
            },
            // Olivia Spark aka Oliff
            new() {
                Id = Guid.Parse("5acc4a80-45e0-4fb6-8c18-76939e8800b4"),
                FirstName = "Olivia",
                LastName = "Spark",
                Alias = "Oliff"
            },
            // Oscar Junior aka Oscar Jr.
            new() {
                Id = Guid.Parse("45fac989-dc43-41c1-8561-c643ee5d0c20"),
                FirstName = "Oscar",
                LastName = "Junior",
                Alias = "Oscar Jr."
            }
        };
    }

    private static List<Publisher> GetPublishersForTest()
    {
        return new List<Publisher>
        {
            // GoldPublish
            new() {
                Id = Guid.Parse("ba8ad35f-c95a-474c-a8e6-245e11339d01"),
                Name = "GoldPublish",
                Country = "Poland",
                City = "Warsaw",
                Address = "Miodowa 1",
                PhoneNumber = "123456789"
            },
            // SilverPublish
            new() {
                Id = Guid.Parse("51fdc79b-6113-45d0-94ea-afe3aad31b9e"),
                Name = "SilverPublish",
                Country = "Poland",
                City = "Warsaw",
                Address = "Miodowa 2",
                PhoneNumber = "987654321"
            },
            // BronzePublish
            new() {
                Id = Guid.Parse("8376f296-f80a-4c8b-b2dd-a6719a80818d"),
                Name = "BronzePublish",
                Country = "Poland",
                City = "Warsaw",
                Address = "Miodowa 3",
                PhoneNumber = "123123123"
            }
        };
    }

    private static List<Book> GetBooksForTest()
    {
        return new List<Book>
        {
            // Test book 1
            new() {
                Id = Guid.Parse("d88ff401-9a0a-4660-a290-ea11ddbe5383"),
                Name = "Test book 1",
                Year = 2022,
                Price = 30,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("17019aeb-10c3-4fd5-92c8-83487985344b"),
                PublisherId = Guid.Parse("ba8ad35f-c95a-474c-a8e6-245e11339d01")
            },
            // Test book 2
            new() {
                Id = Guid.Parse("0529bd79-3660-4615-919b-38ee030c50c2"),
                Name = "Test book 2",
                Year = 2024,
                Price = 51,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("5acc4a80-45e0-4fb6-8c18-76939e8800b4"),
                PublisherId = Guid.Parse("51fdc79b-6113-45d0-94ea-afe3aad31b9e")
            },
            // Test book 3
            new() {
                Id = Guid.Parse("7443ee5a-29d9-423a-a117-4bbbbe37c93b"),
                Name = "Test book 3",
                Year = 2019,
                Price = 15,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("45fac989-dc43-41c1-8561-c643ee5d0c20"),
                PublisherId = Guid.Parse("8376f296-f80a-4c8b-b2dd-a6719a80818d")
            }
        };
    }

    private static List<Item> GetItemsForTest()
    {
        return new List<Item>
        {
            // Test book 1 x3
            new() {
                Id = Guid.Parse("88dcebc6-5665-4b8c-8108-387cc2437e14"),
                BookId = Guid.Parse("d88ff401-9a0a-4660-a290-ea11ddbe5383")
            },
            new() {
                Id = Guid.Parse("1821ea2c-1041-4c31-90ce-b3978d262a9c"),
                BookId = Guid.Parse("d88ff401-9a0a-4660-a290-ea11ddbe5383")
            },
            new() {
                Id = Guid.Parse("0b7b2cd6-1dcb-449b-ab64-38bc3eb4e039"),
                BookId = Guid.Parse("d88ff401-9a0a-4660-a290-ea11ddbe5383")
            },
            // Test book 2 x3
            new() {
                Id = Guid.Parse("98656f53-f492-4239-9109-1668b15d53ef"),
                BookId = Guid.Parse("0529bd79-3660-4615-919b-38ee030c50c2")
            },
            new() {
                Id = Guid.Parse("78e5b4b7-c47f-453d-b734-a95ae2d7e0f2"),
                BookId = Guid.Parse("0529bd79-3660-4615-919b-38ee030c50c2")
            },
            new() {
                Id = Guid.Parse("8bddcf59-df2b-466e-92f9-8095302ce499"),
                BookId = Guid.Parse("0529bd79-3660-4615-919b-38ee030c50c2")
            },
            // Test book 3 x3
            new() {
                Id = Guid.Parse("5a53c968-8912-4f85-8a8f-60d0b5a9b0b8"),
                BookId = Guid.Parse("7443ee5a-29d9-423a-a117-4bbbbe37c93b")
            },
            new() {
                Id = Guid.Parse("4610b679-5c63-4d58-a1c8-2fa7e0f091d3"),
                BookId = Guid.Parse("7443ee5a-29d9-423a-a117-4bbbbe37c93b")
            },
            new() {
                Id = Guid.Parse("d7dc4545-eae5-4004-80a2-564b764d2a97"),
                BookId = Guid.Parse("7443ee5a-29d9-423a-a117-4bbbbe37c93b")
            }
        };
    }
}
