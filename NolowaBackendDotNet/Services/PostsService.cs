using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
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
    public interface IPostsService
    {
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(long userId);
        IEnumerable<PostDTO> GetFollowerPosts(AccountDTO loginedUserAccount);
        Post InsertPost(Post post);
    }

    public class PostsService : ServiceBase<PostsService>, IPostsService
    {
        public PostsService()
        {
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

        public IEnumerable<PostDTO> GetFollowerPosts(AccountDTO loginedUserAccount)
        {
            var followerIds = new List<long>();

            followerIds.Add(loginedUserAccount.Id);
            followerIds.AddRange(loginedUserAccount.Followers.Select(x => x.Id));

            var followersPosts = _context.Posts.Where(x => followerIds.Contains(x.AccountId))
                                               .Include(x => x.Account)
                                               .ThenInclude(x => x.ProfileInfo)
                                               .ThenInclude(x => x.ProfileImg)
                                               .AsEnumerable();

            return _mapper.Map<List<PostDTO>>(followersPosts);
        }

        public Post InsertPost(Post post)
        {
            try
            {
                post.AccountId = post.Account.Id;
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
    }
}
