using NolowaBackendDotNet.Context;
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
        IEnumerable<Post> GetFollowerPosts(AccountDTO loginedUserAccount);
    }

    public class PostsService : IPostsService
    {
        private readonly NolowaContext _context;

        public PostsService(NolowaContext context)
        {
            _context = context;
        }

        public IEnumerable<Post> GetFollowerPosts(AccountDTO loginedUserAccount)
        {
            var followerIds = new List<long>() {
                loginedUserAccount.Id,
            };

            foreach (var item in loginedUserAccount.Followers)
            {
                followerIds.Add(item.Id);
            }

            var followersPosts = _context.Posts.Where(x => followerIds.Contains(x.Id));

            return followersPosts;
        }
    }
}
