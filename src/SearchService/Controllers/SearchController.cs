using System;
using System.Text.Json;
using AutoMapper;
using BookService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SearchService.DTOs;
using SearchService.Entities;
using SearchService.Helpers;
using SearchService.Interfaces;

namespace SearchService.Controllers;

public class SearchController(ISearchRepository searchRepository, IDistributedCache cache,
    IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        string searchParamsJson = JsonSerializer.Serialize(searchParams);
        string cacheKey = $"Items_{searchParamsJson}";
        var cachedData = await RedisCacheHelper.GetFromCacheAsync<SearchDto>(cache, cacheKey);
        if (cachedData != null)
        {
            return Ok(cachedData);
        }

        var result = await searchRepository.GetBooks(searchParams);
        var resultDto = mapper.Map<SearchDto>(result);
        
        await RedisCacheHelper.SetToCacheAsync(cache, cacheKey, resultDto);
        
        return Ok(resultDto);
    }
}
