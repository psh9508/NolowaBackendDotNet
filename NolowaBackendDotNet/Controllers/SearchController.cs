using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Services;
using SharedLib.Dynamodb.Models;
using System.Collections.Generic;
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
        [Authorize]
        public async Task<List<string>> GetSearchedKeywordsAsync(long userId)
        {
            return await _serachService.GetSearchedKeywordsAsync(userId);
        }
        
        [HttpGet("Keywords/Rank/{startIndex}/{endIndex}")]
        public async Task<List<ScoreInfo>> GetKeywordRank(int startIndex, int endIndex)
        {
            return await _serachService.GetSearchkeywordRankAsync(startIndex, endIndex);
        }


        [HttpGet("User/{accountName}")]
        public async Task<List<DdbUser>> SearchUsers(string accountName)
        {
            var accountID = GetLoggedInUserAccountIDFromToken();

            return await _serachService.SearchUsersAsync(accountID, accountName);
        }
    }
}
