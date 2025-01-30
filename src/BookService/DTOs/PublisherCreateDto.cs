using System.ComponentModel.DataAnnotations;

namespace BookService.DTOs;

public class PublisherCreateDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Country { get; set; }

    [Required]
    public required string City { get; set; }

    [Required]
    public required string Address { get; set; }

    [Required]
    public required string PhoneNumber { get; set; }
    public string? PageUrl { get; set; }
}
