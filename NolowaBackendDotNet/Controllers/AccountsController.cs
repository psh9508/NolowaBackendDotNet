using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly NolowaContext _context;

        public AccountsController(NolowaContext context)
        {
            _context = context;
        }

        [HttpGet("Alive")]
        public ActionResult Alive()
        {
            return Ok();
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // GET: Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(long id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(long id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(long id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Login")]
        public ActionResult<Account> Login([FromBody]JsonElement jsonData)
        {
            var id = jsonData.SafeGetProperty("id").GetString();
            var password = jsonData.SafeGetProperty("password").GetString();

            var account = GetAccount(id, password);

            if (account == null)
                return null;

            return Ok(account);

            //String id = param.get("id");
            //String password = param.get("password");

            //var account = authenticationService.login(id, password);

            //if (account == null)
            //    return null;

            //// Follower setting


            //ProfileImageHelper.setDefaultProfileFile(account);

            //account.setJwtToken(jwtTokenProvider.generateToken(account.getEmail()));

            //return account;
        }

        private Account GetAccount(string email, string password)
        {
            var account = _context.Accounts.Where(x => x.Email == email && x.Password == password)
                                    .Include(account => account.FollowerDestinationAccounts)
                                    .Include(account => account.FollowerSourceAccounts)
                                    .Include(account => account.Posts.OrderByDescending(x => x.InsertDate).Take(10))
                                    .FirstOrDefault() ?? throw new Exception("로그인 실패");

            return account;
        }

        private bool AccountExists(long id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
