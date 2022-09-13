using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Test;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface ISearchService
    {
        Task<List<string>> GetSearchedKeywordsAsync(long userID);
        Task<List<AccountDTO>> SearchUsersAsync(long searchAccountID, string accountName);
        Task<List<ScoreInfo>> GetSearchkeywordRankAsync(int startIndex = 0, int endIndex = 5);
    }

    public class SearchService : ServiceBase<SearchService>, ISearchService
    {
        private const int MAX_SEARCH_COUNT = 5;
        private readonly ISearchCacheService _cache;

        public SearchService(NolowaContext context, IMapper mapper, ISearchCacheService cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<string>> GetSearchedKeywordsAsync(long accountId)
        {
            // 여기서 가져온 후 삭제하는 것 보다 저장할 때 5개 이상인 것은 지우고 저장해야 할듯.
            var searchedKeywords = _context.SearchHistories.Where(x => x.AccountId == accountId)
                                                           .Select(x => x.Keyword);

            return await searchedKeywords.ToListAsync();
        }

        public async Task<List<AccountDTO>> SearchUsersAsync(long userID, string accountName)
        {
            // 대소문자 무시하고 비교
            var searchedUsers = _context.Accounts.Where(x => x.AccountName.Contains(accountName))
                                                 .Include(x => x.ProfileInfo)
                                                 .ThenInclude(x => x.ProfileImg)
                                                 .Select(x => _mapper.Map<AccountDTO>(x));

            // 다른 쓰레드로 키워드 검색 될 때마다 Redis에 점수를 올려 순위를 기록한다.
            // 이 쓰레드는 리턴을 기다리지 않고 다음 로직을 탄다.
            _ = _cache.IncreaseScoreAsync(accountName);

            await DeleteAndSaveKeywordAsync(userID, accountName);

            return await searchedUsers.ToListAsync();
        }

        public async Task<List<ScoreInfo>> GetSearchkeywordRankAsync(int startIndex = 0, int endIndex = 5)
        {
            return await Task.Run(() =>
            {
                var ranking = _cache.GetTopRanking(startIndex, endIndex);

                return ranking.ToList();
            });
        }

        private async Task DeleteAndSaveKeywordAsync(long id, string keyword)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await RemoveSameKeyword(id, keyword);
                await SaveNewKeyword(id, keyword);
                await RemoveRowsExceedMaxCountAsync(id);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // we should tell'em that the request you give me is failed.
            }
        }

        private async Task RemoveSameKeyword(long id, string keyword)
        {
            var sameKeywords = _context.SearchHistories.Where(x => x.AccountId == id && x.Keyword == keyword);

            if (sameKeywords.Count() > 0)
            {
                foreach (var sameKeyword in sameKeywords)
                    _context.SearchHistories.Remove(sameKeyword);
                
                await _context.SaveChangesAsync();
            }
        }

        private async Task SaveNewKeyword(long id, string keyword)
        {
            _context.SearchHistories.Add(new SearchHistory()
            {
                AccountId = id,
                Keyword = keyword,
            });

            await _context.SaveChangesAsync();
        }

        private async Task RemoveRowsExceedMaxCountAsync(long id)
        {
            int deletedRowCount = GetRowCountExceedMaxCount(id);

            if (deletedRowCount > 0)
            {
                var deletedRows = _context.SearchHistories.OrderBy(x => x).Take(deletedRowCount);
                _context.SearchHistories.RemoveRange(deletedRows);

                await _context.SaveChangesAsync();
            }
        }

        private int GetRowCountExceedMaxCount(long id)
        {
            return _context.SearchHistories.Where(x => x.AccountId == id).Count() - MAX_SEARCH_COUNT;
        }
    }
}
