using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostsController : NolowaController
    {
        private readonly IAccountsService _accountsService;
        private readonly IPostsService _postsService;

        public PostsController(IAccountsService accountsService, IPostsService postsService)
        {
            _accountsService = accountsService;
            _postsService = postsService;
        }

        [HttpGet("{loginUserId}/HomePosts/")]
        public async Task<ActionResult<IEnumerable<Post>>> GetHomePosts(int loginUserId)
        {
            var logindedUser = await _accountsService.FindAsync(loginUserId);

            var posts = await _postsService.GetHomePostsAsync(logindedUser);

            if (posts.Count() <= 0)
                return NotFound(posts);

            return Ok(posts);
        }

        [HttpGet("{loginUserId}/Followers/{pageNumber}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetMoreHomePostsAsync(int loginUserId, int pageNumber)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException("pageNumber는 0보다 작을 수 없습니다.");

            var logindedUser = await _accountsService.FindAsync(loginUserId);

            var posts = await _postsService.GetMoreHomePostsAsync(logindedUser, pageNumber);

            if (posts.Count() <= 0)
                return NotFound(posts);

            return Ok(posts);
        }

        [HttpPost("New")]
        public async Task<ActionResult<Post>> InsertNewPost([FromBody] Post newPost)
        {
            return await Task.Run(() =>
            {
                return _postsService.InsertPost(newPost);
            });
        }

        [HttpGet("{userId}/Posts")]
        public async Task<ActionResult<IEnumerable<Post>>> GetUserPosts(int userId)
        {
            var userPosts = await _postsService.GetUserPostsAsync(userId);

            return Ok(userPosts);
        }
    }
}
