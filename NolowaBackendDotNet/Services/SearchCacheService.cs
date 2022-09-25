using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Redis;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services.Base;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface ISearchCacheService
    {
        Task IncreaseScoreAsync(string userId, string key, int value = 1);
        IEnumerable<ScoreInfo> GetTopRanking(int start = 0, int end = 5);
    }

    public class ScoreInfo
    {
        public string Key { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class SearchCacheService : ISearchCacheService
    {
        private const string RANK_KEY = "search-rank";
        private readonly ISearchRedis _cache;
        private readonly ConnectionMultiplexer _redis;

        public SearchCacheService(ISearchRedis cache, IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("ConnectionStrings:Redis_Search"));
            _cache = cache;
        }

        public async Task IncreaseScoreAsync(string userId, string key, int value = 1)
        {
            await Task.Run(async () =>
            {
                string userAndKeywordKey = $"{userId}_{key}";

                IDatabase db = _redis.GetDatabase();

                bool userWhoHasSearchedTheSameKeyword = (await db.StringGetAsync(userAndKeywordKey)).HasValue;

                if (userWhoHasSearchedTheSameKeyword)
                    return;

                // 함수가 호출 될 때마다 1씩 올린다.
                _ = db.SortedSetIncrementAsync(RANK_KEY, key, value);

                // 검색했던 유저와 키워드를 저장해 놓는다. (1시간 후 지워지는 데이터)
                // 1시간 동안 같은 검색어를 같은 유저가 검색할 수 없도록 한다.
                _ = db.StringSetAsync(userAndKeywordKey, 1, TimeSpan.FromHours(1));
            });
        }

        public IEnumerable<ScoreInfo> GetTopRanking(int start = 0, int end = 5)
        {
            IDatabase db = _redis.GetDatabase();

            var redisDatas = db.SortedSetRangeByRankWithScores(RANK_KEY, start, end, Order.Descending);

            for (int i = 0; i < redisDatas.Length; ++i)
            {
                yield return new ScoreInfo
                {
                    Key = redisDatas[i].Element,
                    Score = (int)redisDatas[i].Score,
                };
            }
        }
    }
}
