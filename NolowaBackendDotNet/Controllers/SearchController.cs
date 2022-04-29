using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : NolowaController
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

        [HttpGet("User/{accountName}")]
        public async Task<List<AccountDTO>> SearchUsers(string accountName)
        {
            var accountID = GetLoggedInUserAccountIDFromToken();

            return await _serachService.SearchUsersAsync(accountID, accountName);
        }
    }
}
