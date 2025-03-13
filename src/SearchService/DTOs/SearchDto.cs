using SearchService.Entities;

namespace SearchService.DTOs;

public class SearchDto
{
    public IReadOnlyList<Book> Results { get; set; } = [];
    public int PageCount { get; set; }
    public long TotalCount { get; set; }
}
