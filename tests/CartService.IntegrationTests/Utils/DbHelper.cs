using CartService.Data;
using CartService.Entities;

namespace CartService.IntegrationTests.Utils;

public static class DbHelper
{
    public static void InitDbForTests(CartDbContext context)
    {
        context.Carts.AddRange(GetCartsForTest());
        context.Books.AddRange(GetBooksForTest());
        context.BookCarts.AddRange(GetBookCartsForTest());
        context.SaveChanges();
    }

    public static void ReinitDbForTests(CartDbContext context)
    {
        context.Carts.RemoveRange(context.Carts);
        context.Books.RemoveRange(context.Books);
        context.BookCarts.RemoveRange(context.BookCarts);
        context.SaveChanges();
        InitDbForTests(context);
    }

    private static List<Cart> GetCartsForTest()
    {
        return new List<Cart>
        {
            // Cart 1
            new() {
                Id = Guid.Parse("d3f14d6a-2278-4275-998f-0b6db4905074"),
                Username = "bob",
                TotalPrice = 150,
                Status = CartStatus.Active
            },
            // Cart 2
            new() {
                Id = Guid.Parse("fe26a307-0d67-4308-a27f-5c20bf2c194c"),
                Username = "tob",
                TotalPrice = 150,
                Status = CartStatus.Proceeding
            },
            // Cart 3
            new() {
                Id = Guid.Parse("5f6123a8-e265-40cf-9794-5121c5dde9c5"),
                Username = "tom",
                TotalPrice = 150,
                Status = CartStatus.Finished
            },
        };
    }

    private static List<Book> GetBooksForTest()
    {
        return new List<Book>
        {
            // Book 1
            new() {
                Id = Guid.Parse("ccd7f7eb-6684-4fab-bdc3-05006b42087c"),
                Name = "Test book 1",
                Year = 2022,
                Price = 30,
                Items = 10,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("17019aeb-10c3-4fd5-92c8-83487985344b"),
                AuthorFirstName = "Test first name 1",
                AuthorLastName = "Test last name 1",
                AuthorAlias = "Test alias 1",
                PublisherId = Guid.Parse("ba8ad35f-c95a-474c-a8e6-245e11339d01"),
                PublisherName = "TestPublisher1",
                PublisherCountry = "Poland",
                PublisherCity = "Warsaw",
                PublisherAddress = "Kwiatowa 1",
                PublisherPageUrl = "some.url",
                PublisherPhoneNumber = "123456543"
            },
            // Book 2
            new() {
                Id = Guid.Parse("b81136c2-0745-4006-9709-aaf829d7c662"),
                Name = "Test book 2",
                Year = 2024,
                Price = 51,
                Items = 10,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("5acc4a80-45e0-4fb6-8c18-76939e8800b4"),
                AuthorFirstName = "Test first name 2",
                AuthorLastName = "Test last name 2",
                AuthorAlias = "Test alias 2",
                PublisherId = Guid.Parse("51fdc79b-6113-45d0-94ea-afe3aad31b9e"),
                PublisherName = "TestPublisher2",
                PublisherCountry = "Poland",
                PublisherCity = "Warsaw",
                PublisherAddress = "Kwiatowa 2",
                PublisherPageUrl = "some.url",
                PublisherPhoneNumber = "123456543"
            },
            // Book 3
            new() {
                Id = Guid.Parse("d91dc806-1fea-4023-ab6c-c6f07a3c5c66"),
                Name = "Test book 3",
                Year = 2019,
                Price = 15,
                Items = 10,
                ImageUrl = "randomImage.url",
                AuthorId = Guid.Parse("45fac989-dc43-41c1-8561-c643ee5d0c20"),
                AuthorFirstName = "Test first name 3",
                AuthorLastName = "Test last name 3",
                AuthorAlias = "Test alias 3",
                PublisherId = Guid.Parse("8376f296-f80a-4c8b-b2dd-a6719a80818d"),
                PublisherName = "TestPublisher3",
                PublisherCountry = "Poland",
                PublisherCity = "Warsaw",
                PublisherAddress = "Kwiatowa 3",
                PublisherPageUrl = "some.url",
                PublisherPhoneNumber = "123456543"
            }
        };
    }

    private static List<BookCart> GetBookCartsForTest()
    {
        return new List<BookCart>
        {
            // Cart 1, book 1 x3
            new() {
                Id = Guid.Parse("d13f8275-2931-401a-827e-006b3e79e2fb"),
                Quantity = 3,
                BookId = Guid.Parse("ccd7f7eb-6684-4fab-bdc3-05006b42087c"),
                CartId = Guid.Parse("d3f14d6a-2278-4275-998f-0b6db4905074")
            },
            // Cart 2, book 2 x3
            new() {
                Id = Guid.Parse("0cbd01dd-d5f2-416a-8fa0-f1a9815d116f"),
                Quantity = 3,
                BookId = Guid.Parse("b81136c2-0745-4006-9709-aaf829d7c662"),
                CartId = Guid.Parse("fe26a307-0d67-4308-a27f-5c20bf2c194c")
            },
            // Cart 3, book 3 x3
            new() {
                Id = Guid.Parse("3d232f48-b29a-4233-9f79-1b68633d5b61"),
                Quantity = 3,
                BookId = Guid.Parse("d91dc806-1fea-4023-ab6c-c6f07a3c5c66"),
                CartId = Guid.Parse("5f6123a8-e265-40cf-9794-5121c5dde9c5")
            },
        };
    }
}
