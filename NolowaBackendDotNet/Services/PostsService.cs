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
            //var followerIds = new ArrayList<Long>();

            //followerIds.add(user.getId());

            ////        user.getFollowers().stream().map(x -> x.getFollowerUser().getId()).forEach(x -> {
            ////            followerIds.add(x); // It's not thread-safe.
            ////        });

            //for (var follower : user.getFollowers())
            //{
            //    followerIds.add(follower.getFollowerUser().getId());
            //}

            return _context.Posts.Where(x => x.Id == 2);
        }
    }
}
