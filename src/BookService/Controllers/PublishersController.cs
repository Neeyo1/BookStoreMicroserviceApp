using AutoMapper;
using BookService.DTOs;
using BookService.Entities;
using BookService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers;

public class PublishersController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()
    {
        var publishers = await unitOfWork.PublisherRepository.GetPublishersAsync();
        
        return Ok(publishers);
    }

    [HttpGet("{publisherId}")]
    public async Task<ActionResult<PublisherDto>> GetPublisher(Guid publisherId)
    {
        var publisher = await unitOfWork.PublisherRepository.GetPublisherByIdAsync(publisherId);
        if (publisher == null) return NotFound();

        return Ok(mapper.Map<PublisherDto>(publisher));
    }

    [HttpPost]
    public async Task<ActionResult<PublisherDto>> CreatePublisher(PublisherCreateDto publisherCreateDto)
    {
        var publisher = mapper.Map<Publisher>(publisherCreateDto);

        unitOfWork.PublisherRepository.AddPublisher(publisher);

        var newPublisher = mapper.Map<PublisherDto>(publisher);

        if (await unitOfWork.Complete())
            return CreatedAtAction(nameof(GetPublisher), new {publisherId = publisher.Id}, newPublisher);
            
        return BadRequest("Failed to create publisher");
    }

    [HttpPut("{publisherId}")]
    public async Task<ActionResult> UpdatetPublisher(PublisherUpdateDto publisherUpdateDto, Guid publisherId)
    {
        var publisher = await unitOfWork.PublisherRepository.GetPublisherByIdAsync(publisherId);
        if (publisher == null) return BadRequest("Failed to find publisher");

        publisher.Name = publisherUpdateDto.Name ?? publisher.Name;
        publisher.Country = publisherUpdateDto.Country ?? publisher.Country;
        publisher.City = publisherUpdateDto.City ?? publisher.City;
        publisher.Address = publisherUpdateDto.Address ?? publisher.Address;
        publisher.PhoneNumber = publisherUpdateDto.PhoneNumber ?? publisher.PhoneNumber;
        publisher.PageUrl = publisherUpdateDto.PageUrl ?? publisher.PageUrl;

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update publisher");
    }

    [HttpDelete("{publisherId}")]
    public async Task<ActionResult> DeletePublisher(Guid publisherId)
    {  
        var publisher = await unitOfWork.PublisherRepository.GetPublisherByIdAsync(publisherId);
        if (publisher == null) return BadRequest("Failed to find publisher");

        unitOfWork.PublisherRepository.DeletePublisher(publisher);

        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to delete publisher");
    }
}
