using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
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
    }

    public class SearchService : ServiceBase<SearchService>, ISearchService
    {
        private const int MAX_SEARCH_COUNT = 5;

        public SearchService()
        {
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
                                                 .Include(x => x.ProfileImage)
                                                 .Select(x => _mapper.Map<AccountDTO>(x));

            await DeleteAndSaveKeywordAsync(userID, accountName);

            return await searchedUsers.ToListAsync();
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
            _context.SearchHistories.Add(new Models.SearchHistory()
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
                var deletedRows = _context.SearchHistories.OrderByDescending(x => x).Take(deletedRowCount);
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
