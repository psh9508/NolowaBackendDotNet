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
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface ISearchCacheService
    {
        void IncreaseScore(string key, int value = 1);
        IEnumerable<ScoreInfo> GetTopRanking(int start = 0, int end = 5);
    }

    public class ScoreInfo
    {
        public string Key { get; set; }
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

        public void IncreaseScore(string key, int value = 1)
        {
            IDatabase db = _redis.GetDatabase();

            // 함수가 호출 될 때마다 1씩 올린다.
            db.SortedSetIncrement(RANK_KEY, key, value);
        }

        public IEnumerable<ScoreInfo> GetTopRanking(int start = 0, int end = 5)
        {
            IDatabase db = _redis.GetDatabase();

            var list = db.SortedSetRangeByRankWithScores(RANK_KEY, start, end, Order.Descending);

            var ranks = new List<ScoreInfo>();

            for (int i = 0; i < list.Length; ++i)
            {
                ranks.Add(new ScoreInfo { 
                    Key = list[i].Element,
                    Score = (int)list[i].Score,
                });
            }

            return ranks;
        }
    }
}
