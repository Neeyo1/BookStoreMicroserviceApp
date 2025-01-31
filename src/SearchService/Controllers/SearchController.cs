using System;
using BookService.Controllers;
using Microsoft.AspNetCore.Mvc;
using SearchService.Entities;
using SearchService.Helpers;
using SearchService.Interfaces;

namespace SearchService.Controllers;

public class SearchController(ISearchRepository searchRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var result = await searchRepository.GetBooks(searchParams);
        
        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
