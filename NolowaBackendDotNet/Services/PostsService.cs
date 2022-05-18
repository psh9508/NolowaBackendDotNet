using AutoMapper;
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
        IEnumerable<PostDTO> GetFollowerPosts(AccountDTO loginedUserAccount);
        Post InsertPost(Post post);
    }

    public class PostsService : ServiceBase<PostsService>, IPostsService
    {
        public PostsService()
        {
        }

        public IEnumerable<PostDTO> GetFollowerPosts(AccountDTO loginedUserAccount)
        {
            var followerIds = new List<long>();

            followerIds.Add(loginedUserAccount.Id);
            followerIds.AddRange(loginedUserAccount.Followers.Select(x => x.Id));

            var followersPosts = _context.Posts.Where(x => followerIds.Contains(x.AccountId)).AsEnumerable();

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
