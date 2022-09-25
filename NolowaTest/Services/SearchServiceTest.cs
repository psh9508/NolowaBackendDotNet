using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Mapper;
using NolowaBackendDotNet.Core.Test;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Services;
using Stenn.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;

namespace NolowaTest.Services
{
    public class SearchServiceTest
    {
        private SearchService _searchService;
        private SearchCacheMock _searchCacheMock;

        [SetUp]
        public void Setup()
        {
            TestHelper.IsTest = true;

            var returnData = Get기본Account데이터();

            var mockSet = new Mock<DbSet<Account>>();

            mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(returnData.Provider);
            mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(returnData.Expression);
            mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(returnData.ElementType);
            mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(returnData.GetEnumerator());

            var returnSearchHistories = Get기본SearchHistory데이터();
            var mockSearchHistoriesSet = new Mock<DbSet<SearchHistory>>();

            mockSearchHistoriesSet.As<IQueryable<SearchHistory>>().Setup(m => m.Provider).Returns(returnSearchHistories.Provider);
            mockSearchHistoriesSet.As<IQueryable<SearchHistory>>().Setup(m => m.Expression).Returns(returnSearchHistories.Expression);
            mockSearchHistoriesSet.As<IQueryable<SearchHistory>>().Setup(m => m.ElementType).Returns(returnSearchHistories.ElementType);
            mockSearchHistoriesSet.As<IQueryable<SearchHistory>>().Setup(m => m.GetEnumerator()).Returns(returnSearchHistories.GetEnumerator());

            var mockContext = new Mock<NolowaContext>();

            var databaseMock = new Mock<DatabaseFacade>(mockContext.Object);
            databaseMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseMock.Setup(x => x.BeginTransaction()).Returns(Mock.Of<IDbContextTransaction>());

            mockContext.Setup(x => x.Database).Returns(databaseMock.Object);
            mockContext.Setup(x => x.Accounts).Returns(mockSet.Object);
            mockContext.Setup(x => x.SearchHistories).Returns(mockSearchHistoriesSet.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            _searchCacheMock = new SearchCacheMock();
            _searchService = new SearchService(mockContext.Object, mapper, _searchCacheMock);
        }

        [TearDown]
        public void TearDown()
        {
            TestHelper.IsTest = false;
        }

        [Test]
        public async Task GetSearchedKeywordsAsync_새로운값이올바로레디스에저장된다()
        {
            var keywords = await _searchService.GetSearchedKeywordsAsync(1);

            Assert.That(keywords.Count, Is.EqualTo(8));
            Assert.That(keywords[0], Is.EqualTo("1"));
            Assert.That(keywords[1], Is.EqualTo("2"));
            Assert.That(keywords[2], Is.EqualTo("3"));
            Assert.That(keywords[3], Is.EqualTo("4"));
            Assert.That(keywords[4], Is.EqualTo("5"));
            Assert.That(keywords[5], Is.EqualTo("6"));
            Assert.That(keywords[6], Is.EqualTo("7"));
            Assert.That(keywords[7], Is.EqualTo("8"));
        }

        [Test]
        [TestCase("병아리")]
        [TestCase("계정명")]
        [TestCase("키보드")]
        [TestCase("커피아빠")]
        [TestCase("시골용사")]
        [TestCase("흑생")]
        public async Task SearchUsersAsync_DB에있는값을올바로가져온다(string keyword)
        {
            var searchedData = await _searchService.SearchUsersAsync(It.IsAny<long>(), keyword);

            Assert.That(searchedData[0].AccountName, Is.EqualTo(keyword));
        }

        [Test]
        public async Task SearchUsersAsync_DB에없는값은EmptyList를반환한다()
        {
            string dataNotInDB = "DB에 없는 데이터";

            var searchedData = await _searchService.SearchUsersAsync(It.IsAny<long>(), dataNotInDB);

            Assert.That(searchedData.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// 주의 : 실제 Redis를 이용해서 테스트 하지 않는다.
        ///        Redis를 테스트 할 수 없어 Redis를 가정한 List를 이용해 테스트를 함.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SearchUsersAsync_검색한수만큼점수가올라야한다()
        {
            await _searchService.SearchUsersAsync(It.IsAny<long>(), "계정명");
            await _searchService.SearchUsersAsync(It.IsAny<long>(), "계정명");
            await _searchService.SearchUsersAsync(It.IsAny<long>(), "피곤한거미");

            Assert.That(_searchCacheMock.Datas[0].Key, Is.EqualTo("계정명"));
            Assert.That(_searchCacheMock.Datas[0].Score, Is.EqualTo(2));
            Assert.That(_searchCacheMock.Datas[1].Key, Is.EqualTo("피곤한거미"));
            Assert.That(_searchCacheMock.Datas[1].Score, Is.EqualTo(1));
        }

        /// <summary>
        /// ISearchCacheService를 구현하는 Search Cache의 Mock이다
        /// 내부적으로 Redis에 접근하는 코드를 메모리 데이터에 접근하는 방식으로 변경해서
        /// 테스트할 수 있는 클래스를 만든다.
        /// </summary>
        public class SearchCacheMock : ISearchCacheService
        {
            public class SearchUser
            {
                public string Key { get; set; } = string.Empty;
                public long TTL { get; set; }
            }

            private const string RANK_KEY = "search-rank";

            // Redis 대신 데이터를 담아 Assert 될 메모리 데이터
            public List<ScoreInfo> Datas { get; set; } = new List<ScoreInfo>();
            public List<SearchUser> SearchUsers { get; set; } = new List<SearchUser>();

            public IEnumerable<ScoreInfo> GetTopRanking(int start = 0, int end = 5)
            {
                throw new NotImplementedException();
            }

            public async Task IncreaseScoreAsync(string userId, string key, int value = 1)
            {
                string 검색기록Key = $"{userId}_{key}";

                await Task.Run(() =>
                {
                    // 같은 유저가 같은 검색어로 검색했던 기록이 아직 레디스에 남아있으면 검색어를 랭킹을 올려주지 않음
                    if (SearchUsers.FirstOrDefault(x => x.Key == 검색기록Key) is not null)
                        return;

                    // Redis의 ZINCRBY를 똑같이 구현한다.
                    var data = Datas.FirstOrDefault(x => x.Key == key);

                    if (data is not null)
                    {
                        data.Score += value;
                    }
                    else
                    {
                        Datas.Add(new ScoreInfo() { Key = key, Score = value });
                    }

                    SearchUsers.Add(new SearchUser() { Key = 검색기록Key, TTL = 1000 * 60 * 60 });
                });
            }
        }

        private IQueryable<SearchHistory> Get기본SearchHistory데이터()
        {
            return new List<SearchHistory>()
            {
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "1",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "2",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "3",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "4",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "5",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "6",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "7",
                },
                new SearchHistory()
                {
                    AccountId = 1,
                    Keyword = "8",
                },
                new SearchHistory()
                {
                     AccountId = 2,
                    Keyword = "9",
                },
                new SearchHistory()
                {
                    AccountId = 2,
                    Keyword = "10",
                },
            }.AsAsyncQueryable();
        }

        private IQueryable<Account> Get기본Account데이터()
        {
            return new List<Account>()
            {
                new Account()
                {
                    Id = 1,
                    AccountName = "병아리",
                },
                new Account()
                {
                    Id = 2,
                    AccountName = "계정명",
                },
                new Account()
                {
                    Id = 3,
                    AccountName = "키보드",
                },
                new Account()
                {
                    Id = 4,
                    AccountName = "커피아빠",
                },
                new Account()
                {
                    Id = 5,
                    AccountName = "시골용사",
                },
                new Account()
                {
                    Id = 6,
                    AccountName = "흑생",
                }
            }.AsAsyncQueryable();
        }
    }
}
