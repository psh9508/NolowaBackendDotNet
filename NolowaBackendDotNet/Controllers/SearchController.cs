using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController
    {
        private readonly ISearchService _serachService;

        public SearchController(ISearchService serachService)
        {
            _serachService = serachService;
        }

        [HttpGet("Keywords/{userId}")]
        public async Task<List<string>> GetSearchedKeywordsAsync(long userId)
        {
            return await _serachService.GetSearchedKeywordsAsync(userId);
        }
    }
}
