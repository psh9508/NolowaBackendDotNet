using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Mapper;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaTest.Services
{
    public class PostServiceTest
    {
        PostsService _postService;

        [SetUp]
        public void Setup()
        {
            NolowaBackendDotNet.Core.Test.TestHelper.IsTest = true;

            var 기본데이터 = Get기본테스트데이터();

            SetupReturnData(기본데이터);
        }

        [TearDown]
        public void TearDown()
        {
            NolowaBackendDotNet.Core.Test.TestHelper.IsTest = false;
        }

        /// <summary>
        /// 16개의 데이터가 있다.
        /// Id:1은 15개 Id:3은 1개
        /// 함수를 호출하면 5개씩 post를 차레대로 가져온다.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetMoreHomePostsAsync_여러번호출했을때도올바로다음페이지를가져와야한다()
        {
            // 로그인한 유저
            var account = new AccountDTO()
            {
                Id = 1,
                Followers = new List<FollowerDTO>()
                {
                    new FollowerDTO()
                    {
                        Id = 3, // Id 3번을 팔로우하고있다.
                    },
                }
            };

            // [첫 번째 페이지]
            var page = (await _postService.GetMoreHomePostsAsync(account, 1)).ToList();

            // 16개의 데이터 중 0개 skip 하고 5개 가져온다.
            Assert.That(page.Count, Is.EqualTo(5));
            Assert.That(page[0].Id, Is.EqualTo(23));
            Assert.That(page[1].Id, Is.EqualTo(22));
            Assert.That(page[2].Id, Is.EqualTo(21));
            Assert.That(page[3].Id, Is.EqualTo(20));
            Assert.That(page[4].Id, Is.EqualTo(19));

            // [두 번째 페이지]
            page = (await _postService.GetMoreHomePostsAsync(account, 2)).ToList();

            // 16개의 데이터 중 5개 skip하고 5개 가져온다.
            Assert.That(page.Count, Is.EqualTo(5));
            Assert.That(page[0].Id, Is.EqualTo(18));
            Assert.That(page[1].Id, Is.EqualTo(17));
            Assert.That(page[2].Id, Is.EqualTo(16));
            Assert.That(page[3].Id, Is.EqualTo(15));
            Assert.That(page[4].Id, Is.EqualTo(14));

            // [세 번째 페이지]
            page = (await _postService.GetMoreHomePostsAsync(account, 3)).ToList();

            // 16개의 데이터 중 10개 skip하고 5개 가져온다.
            Assert.That(page.Count, Is.EqualTo(5));
            Assert.That(page[0].Id, Is.EqualTo(13));
            Assert.That(page[1].Id, Is.EqualTo(12));
            Assert.That(page[2].Id, Is.EqualTo(11));
            // 팔로우 하지 않았던 AccountId 2번의 데이터는 가져오지 않는다. //
            Assert.That(page[3].Id, Is.EqualTo(3));
            Assert.That(page[4].Id, Is.EqualTo(2));

            // [네 번째 페이지]
            page = (await _postService.GetMoreHomePostsAsync(account, 4)).ToList();

            // 16개의 데이터 중 15개 skip하고 1개 가져온다.
            Assert.That(page.Count, Is.EqualTo(1));
            Assert.That(page[0].Id, Is.EqualTo(1));

            // [다섯 번째 페이지]
            page = (await _postService.GetMoreHomePostsAsync(account, 5)).ToList();

            // 16개의 데이터 중 16개 skip하고 0개 가져온다.
            Assert.That(page.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(1, 22,21,20,19,18)] // Id 1번의 첫 페이지를 검사한다
        [TestCase(2, 10,9,8,7,6)] // Id 2번의 첫 페이지를 검사한다
        [TestCase(3, 23)] // Id 3번의 첫 페이지를 검사한다
        public async Task GetHomePostsAsync_처음페이지가올바로가져와진다(long id, params long[] postIds)
        {
            var page = (await _postService.GetHomePostsAsync(new AccountDTO()
            {
                Id = id, // 현재 로그인 된 유저의 id를 입력한다.
            })).ToList();

            Assert.That(page.Count, Is.EqualTo(postIds.Length)); // 총 post의 개수를 비교한다.

            for (int i = 0; i < postIds.Length; i++)
            {
                Assert.That(page[i].Id, Is.EqualTo(postIds[i])); // 각 post의 id와 파라미터로 넣은 값을 비교한다.
            }
        }

        [Test]
        [TestCase(1, new long[] { 2 }, 4,3,2,1)]
        [TestCase(1, new long[] { 3 }, 5,2,1)]
        [TestCase(1, new long[] { 2, 3 }, 5,4,3,2,1)]
        [TestCase(2, new long[] { 3 }, 5,4,3)]
        public async Task GetHomePostsAsync_팔로우포스트를포함해처음페이지가올바로가져와진다(long id, long[] followerIds, params long[] postIds)
        {
            var 팔로우테스트데이터 = Get팔로우테스트데이터();

            SetupReturnData(팔로우테스트데이터);

            // 로그인한 유저를 만든다
            var account = new AccountDTO() {
                Id = id,
                Followers = GetFollowers(followerIds),
            };

            var page = (await _postService.GetHomePostsAsync(account)).ToList();

            Assert.That(page.Count, Is.EqualTo(postIds.Length)); // 총 post의 개수를 비교한다.

            for (int i = 0; i < postIds.Length; i++)
            {
                Assert.That(page[i].Id, Is.EqualTo(postIds[i])); // 각 post의 id와 파라미터로 넣은 값을 비교한다.
            }
        }

        private IEnumerable<FollowerDTO> GetFollowers(long[] followerIds)
        {
            for (int i = 0; i < followerIds.Length; i++)
            {
                yield return new FollowerDTO()
                {
                    Id = followerIds[i],
                };
            }
        }

        private IQueryable<Post> Get기본테스트데이터()
        {
            return new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                    AccountId = 1,
                    Message = "1_1",
                    InsertDate = new DateTime(2000,1,1,1,0,0),
                },
                new Post()
                {
                    Id = 2,
                    AccountId = 1,
                    Message = "1_2",
                    InsertDate = new DateTime(2000,1,1,1,0,1),
                },
                new Post()
                {
                    Id = 3,
                    AccountId = 1,
                    Message = "1_3",
                    InsertDate = new DateTime(2000,1,1,1,0,2),
                },
                new Post()
                {
                    Id = 4,
                    AccountId = 2,
                    Message = "2_1",
                    InsertDate = new DateTime(2000,1,1,1,0,3),
                },
                new Post()
                {
                    Id = 5,
                    AccountId = 2,
                    Message = "2_2",
                    InsertDate = new DateTime(2000,1,1,1,0,4),
                },
                new Post()
                {
                    Id = 6,
                    AccountId = 2,
                    Message = "2_3",
                    InsertDate = new DateTime(2000,1,1,1,0,5),
                },
                new Post()
                {
                    Id = 7,
                    AccountId = 2,
                    Message = "2_4",
                    InsertDate = new DateTime(2000,1,1,1,0,6),
                },
                new Post()
                {
                    Id = 8,
                    AccountId = 2,
                    Message = "2_5",
                    InsertDate = new DateTime(2000,1,1,1,0,7),
                },
                new Post()
                {
                    Id = 9,
                    AccountId = 2,
                    Message = "2_6",
                    InsertDate = new DateTime(2000,1,1,1,0,8),
                },
                new Post()
                {
                    Id = 10,
                    AccountId = 2,
                    Message = "2_7",
                    InsertDate = new DateTime(2000,1,1,1,0,9),
                },
                new Post()
                {
                    Id = 11,
                    AccountId = 1,
                    Message = "1_9",
                    InsertDate = new DateTime(2000,1,1,1,0,10),
                },
                new Post()
                {
                    Id = 12,
                    AccountId = 1,
                    Message = "1_10",
                    InsertDate = new DateTime(2000,1,1,1,0,11),
                },
                new Post()
                {
                    Id = 13,
                    AccountId = 1,
                    Message = "1_11",
                    InsertDate = new DateTime(2000,1,1,1,0,12),
                },
                new Post()
                {
                    Id = 14,
                    AccountId = 1,
                    Message = "1_12",
                    InsertDate = new DateTime(2000,1,1,1,0,13),
                },
                new Post()
                {
                    Id = 15,
                    AccountId = 1,
                    Message = "1_13",
                    InsertDate = new DateTime(2000,1,1,1,0,14),
                },
                new Post()
                {
                    Id = 16,
                    AccountId = 1,
                    Message = "1_14",
                    InsertDate = new DateTime(2000,1,1,1,0,15),
                },
                new Post()
                {
                    Id = 17,
                    AccountId = 1,
                    Message = "1_15",
                    InsertDate = new DateTime(2000,1,1,1,0,16),
                },
                new Post()
                {
                    Id = 18,
                    AccountId = 1,
                    Message = "1_16",
                    InsertDate = new DateTime(2000,1,1,1,0,17),
                },
                new Post()
                {
                    Id = 19,
                    AccountId = 1,
                    Message = "1_17",
                    InsertDate = new DateTime(2000,1,1,1,0,18),
                },
                new Post()
                {
                    Id = 20,
                    AccountId = 1,
                    Message = "1_18",
                    InsertDate = new DateTime(2000,1,1,1,0,19),
                },
                new Post()
                {
                    Id = 21,
                    AccountId = 1,
                    Message = "1_19",
                    InsertDate = new DateTime(2000,1,1,1,0,20),
                },
                new Post()
                {
                    Id = 22,
                    AccountId = 1,
                    Message = "1_20",
                    InsertDate = new DateTime(2000,1,1,1,0,21),
                },
                new Post()
                {
                    Id = 23,
                    AccountId = 3,
                    Message = "3_1",
                    InsertDate = new DateTime(2000,1,1,1,0,22),
                },
            }.AsQueryable();
        }

        private IQueryable<Post> Get팔로우테스트데이터()
        {
            return new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                    AccountId = 1,
                    InsertDate = new DateTime(2000,1,1,1,0,0),
                },
                new Post()
                {
                    Id = 2,
                    AccountId = 1,
                    InsertDate = new DateTime(2000,1,1,1,0,1),
                },
                new Post()
                {
                    Id = 3,
                    AccountId = 2,
                    InsertDate = new DateTime(2000,1,1,1,0,2),
                },
                new Post()
                {
                    Id = 4,
                    AccountId = 2,
                    InsertDate = new DateTime(2000,1,1,1,0,3),
                },
                new Post()
                {
                    Id = 5,
                    AccountId = 3,
                    InsertDate = new DateTime(2000,1,1,1,0,4),
                },
            }.AsQueryable();
        }

        private void SetupReturnData(IQueryable<Post> returnData)
        {
            var mockSet = new Mock<DbSet<Post>>();

            mockSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(returnData.Provider);
            mockSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(returnData.Expression);
            mockSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(returnData.ElementType);
            mockSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(returnData.GetEnumerator());

            var mockContext = new Mock<NolowaContext>();
            mockContext.Setup(x => x.Posts).Returns(mockSet.Object);

            var mockCache = new Mock<IPostCacheService>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            _postService = new PostsService(mockContext.Object, mapper, mockCache.Object);
        }
    }
}
