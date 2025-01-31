using System.ComponentModel.DataAnnotations;

namespace BookService.DTOs;

public class BookCreateDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int Year { get; set; }

    [Required]
    public required string ImageUrl { get; set; }

    [Required]
    public required int Price { get; set; }

    [Required]
    public required Guid AuthorId { get; set; }

    [Required]
    public required Guid PublisherId { get; set; }
}
