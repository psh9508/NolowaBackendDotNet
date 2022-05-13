using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : NolowaController
    {
        private readonly NolowaContext _context;
        private readonly IAccountsService _accountsService;

        public AccountsController(NolowaContext context, IAccountsService accountsService)
        {
            _context = context;
            _accountsService = accountsService;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        [HttpPost("Save")]
        public async Task<ActionResult<AccountDTO>> SaveNewAccount([FromBody] Account newAccount)
        {
            var savedAccount = await _accountsService.SaveAsync(newAccount);

            if(savedAccount == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            return Ok(savedAccount);
        }

        // GET: Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountAsync(long id)
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
        public async Task<IActionResult> PutAccountAsync(long id, Account account)
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
        [AllowAnonymous]
        public async Task<ActionResult<Account>> LoginAsync([FromBody] Newtonsoft.Json.Linq.JObject jsonData)
        {
            var id = jsonData.Value<string>("id");
            var password = jsonData.Value<string>("password");

            var account = await _accountsService.LoginAsync(id, password);

            if(account == null)
            {
                NotFound();
            }

            return Ok(account);
        }

        [HttpPost("Follow")]
        public async Task<ActionResult<bool>> ChangeFollowStateAsync([FromBody] IFFollowModel data)
        {
            bool result;

            if (await _accountsService.HasFollowedAsync(data))
                result = await _accountsService.UnFollowAsync(data);
            else
                result = await _accountsService.FollowAsync(data);

            return result ? Ok(result) : BadRequest(result);
        }

        private bool AccountExists(long id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
