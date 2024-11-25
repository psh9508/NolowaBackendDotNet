using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services.Base;
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
    public interface IPostsService
    {
        Task<IEnumerable<PostDTO>> GetHomePostsAsync(AccountDTO loginedUserAccount);
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(long userId);
        Task<IEnumerable<PostDTO>> GetMoreHomePostsAsync(AccountDTO loginedUserAccount, int pageNumber);
        Post InsertPost(Post post);
    }

    public class PostsService : ServiceBase<PostsService>, IPostsService
    {
        private const int PAGE_POST_COUNT = 5;
        private readonly IPostCacheService _cache;

        public PostsService(NolowaContext context, IMapper mapper, IPostCacheService cache, IJWTTokenProvider jwtTokenProvider) : base(jwtTokenProvider)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache; 
        }

        public async Task<IEnumerable<PostDTO>> GetHomePostsAsync(AccountDTO loginedUserAccount)
        {
            // 기존에 캐싱 되어있을지도 모르는 데이터를 삭제
            await _cache.RemoveAllAsync(loginedUserAccount.Id.ToString());

            return await GetMoreHomePostsAsync(loginedUserAccount, pageNumber: 1);
        }

        public async Task<IEnumerable<PostDTO>> GetUserPostsAsync(long userId)
        {
            var dbDatas = await _context.Posts.Where(x => x.AccountId == userId)
                                              .Include(x => x.Account)
                                              .ThenInclude(x => x.ProfileInfo)
                                              .ThenInclude(x => x.ProfileImg)
                                              .OrderByDescending(x => x.InsertDate)
                                              .Take(10)
                                              .AsNoTracking()
                                              .ToListAsync();

            return _mapper.Map<IEnumerable<PostDTO>>(dbDatas);
        }

        public Post InsertPost(Post post)
        {
            try
            {
                post.AccountId = post.Account.Id;
                post.InsertDate = DateTime.Now;
                post.Account = null;
                
                _context.Posts.Add(post);

                _context.SaveChanges();

                return post;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PostDTO>> GetMoreHomePostsAsync(AccountDTO loginedUserAccount, int pageNumber)
        {
            var cachedNextPageData = await _cache.GetAsync<IEnumerable<PostDTO>>(loginedUserAccount.Id.ToString());

            if (cachedNextPageData?.Count() > 0)
            {
                // 다음 페이지 캐싱 하는 쓰레드를 기다리지 않는다.
                _ = GetRequestedPageAndSaveNextPageToCacheAsync(loginedUserAccount, pageNumber);

                // 캐싱 된 데이터를 바로 리턴한다.
                return cachedNextPageData;
            }

            // 캐싱 되어있는 값이 없으면 요청된 페이지를 리턴하고 그 다음페이지를 캐시하는 함수를 호출
            var requestedPagePosts = await GetRequestedPageAndSaveNextPageToCacheAsync(loginedUserAccount, pageNumber);

            // 다음페이지 리턴
            return requestedPagePosts;
        }

        private async Task<IEnumerable<PostDTO>> GetRequestedPageAndSaveNextPageToCacheAsync(AccountDTO loginedUserAccount, int pageNumber)
        {
            var followerIds = new List<long>();

            followerIds.Add(loginedUserAccount.Id);
            followerIds.AddRange(loginedUserAccount.Followers.Select(x => x.Id));

            var followersPosts = _context.Posts.Where(x => followerIds.Contains(x.AccountId))
                                               .Include(x => x.Account)
                                               .ThenInclude(x => x.ProfileInfo)
                                               .ThenInclude(x => x.ProfileImg)
                                               .OrderByDescending(x => x.InsertDate)
                                               .Skip((pageNumber - 1) * PAGE_POST_COUNT)
                                               .Take(PAGE_POST_COUNT * 2) // 리턴할 데이터와 캐시할 데이터 모두 한번에 가져온다.
                                               .Select(x => _mapper.Map<PostDTO>(x))
                                               .AsEnumerable();

            var requestedPagePosts = followersPosts.Take(PAGE_POST_COUNT); // 요청 페이지
            var cachePagePosts = followersPosts.Skip(PAGE_POST_COUNT).Take(PAGE_POST_COUNT); // 캐시 될 요청 페이지의 다음 페이지

            await SaveToCache(loginedUserAccount.Id.ToString(), cachePagePosts); // 페이지 캐시

            return requestedPagePosts;
        }

        private async Task SaveToCache(string loginedUserId, IEnumerable<PostDTO> cachePagePosts)
        {
            // 데이터를 캐시에 삽입하기 전에 타이밍 문제로 캐시에 늦게 올라간데이터가 있을 수 있으므로 캐시의 데이터를 모두 삭제한다.
            await _cache.RemoveAllAsync(loginedUserId);

            var jsonData = JsonSerializer.Serialize(cachePagePosts, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            // 다른 쓰레드를 이용해서 레디스에 캐시하고 이 쓰레드를 기다리지 않는다.
            _ = _cache.SaveAsync(loginedUserId, jsonData);
        }
    }
}
