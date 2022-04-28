using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface ISearchService
    {
        Task<List<string>> GetSearchedKeywordsAsync(long userId);
        Task<List<AccountDTO>> SearchUsersAsync(string accountName);
    }

    public class SearchService : ISearchService
    {
        private const int MAX_SEARCH_COUNT = 5;

        private readonly NolowaContext _context;

        public SearchService(NolowaContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetSearchedKeywordsAsync(long accountId)
        {
            // 여기서 가져온 후 삭제하는 것 보다 저장할 때 5개 이상인 것은 지우고 저장해야 할듯.
            var searchedKeywords = _context.SearchHistories.Where(x => x.AccountId == accountId)
                                                           .Select(x => x.Keyword);

            return await searchedKeywords.ToListAsync();
        }

        public async Task<List<AccountDTO>> SearchUsersAsync(string accountName)
        {
            // 대소문자 무시하고 비교
            var searchedUsers = _context.Accounts.Where(x => x.AccountName.Contains(accountName))
                                                 .Include(x => x.ProfileImage)
                                                 .Select(x => x.ToDTO());

            return await searchedUsers.ToListAsync();
        }
    }
}
