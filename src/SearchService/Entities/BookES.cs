using Nest;

namespace SearchService.Entities;

public class BookES
{
    [Keyword]
    public required string Id { get; set; }

    [Text]
    public required string Name { get; set; }

    [Text]
    public string? AuthorAlias { get; set; }
}
