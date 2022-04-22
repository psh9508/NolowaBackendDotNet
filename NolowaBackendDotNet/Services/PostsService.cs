using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
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

    public class PostsService : IPostsService
    {
        private readonly NolowaContext _context;

        public PostsService(NolowaContext context)
        {
            _context = context;
        }

        public IEnumerable<PostDTO> GetFollowerPosts(AccountDTO loginedUserAccount)
        {
            var followerIds = new List<long>() {
                loginedUserAccount.Id,
            };

            foreach (var item in loginedUserAccount.Followers)
            {
                followerIds.Add(item.Id);
            }

            var followersPosts = _context.Posts.Where(x => followerIds.Contains(x.AccountId));

            return followersPosts.Select(x => x.ToDTO());
        }

        public Post InsertPost(Post post)
        {
            try
            {
                _context.Posts.Add(post);
                return post;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
