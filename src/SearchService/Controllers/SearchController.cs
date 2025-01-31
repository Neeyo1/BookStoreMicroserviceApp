using System;
using BookService.Controllers;
using Microsoft.AspNetCore.Mvc;
using SearchService.Entities;
using SearchService.Interfaces;

namespace SearchService.Controllers;

public class SearchController(ISearchRepository searchRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> SearchItems(string? searchTerm)
    {
        var books = await searchRepository.GetBooks(searchTerm);
        
        return Ok(books);
    }
}
