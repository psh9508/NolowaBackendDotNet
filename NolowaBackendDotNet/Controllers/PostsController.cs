using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
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

        [HttpGet("Alive")]
        public ActionResult Alive()
        {
            return Ok();
        }

        [HttpGet("{loginUserId}/Followers")]
        public async Task<ActionResult<IEnumerable<Post>>> GetFollowerPosts(int loginUserId)
        {
            var logindedUser = await _accountsService.FindAsync(loginUserId);

            var posts = _postsService.GetFollowerPosts(logindedUser);

            if (posts.Count() <= 0)
                return NotFound();

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
    }
}
