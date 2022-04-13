using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAccountsService
    {
        Task<AccountDTO> FindAsync(long id);
        Task<AccountDTO> LoginAsync(string email, string password);
    }

    public class AccountsService : IAccountsService
    {
        private readonly NolowaContext _context;
        private readonly IJWTTokenProvider _jwtTokenProvider;

        public AccountsService(NolowaContext context, IJWTTokenProvider jwtTokenProvider)
        {
            _context = context;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<AccountDTO> FindAsync(long id)
        {
            return await FindAsync(x => x.Id == id);
        }

        public async Task<AccountDTO> LoginAsync(string email, string password)
        {
            var accountDTO = await FindAsync(x => x.Email == email && x.Password == password);

            if (accountDTO == null)
                return null;

            accountDTO.JWTToken = _jwtTokenProvider.GenerateJWTToken(accountDTO);

            return accountDTO;

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

        private async Task<AccountDTO> FindAsync(Expression<Func<Account, bool>> whereExpression)
        {
            var account = await _context.Accounts.Where(whereExpression)
                                               //.Include(account => account.FollowerDestinationAccounts)
                                               //.Include(account => account.FollowerSourceAccounts)
                                               //.Include(account => account.Posts.OrderByDescending(x => x.InsertDate).Take(10))
                                               .Include(account => account.ProfileImage)
                                               .FirstOrDefaultAsync();
            if (account == null)
                return null;

            foreach (var follower in _context.Followers.Where(x => x.SourceAccountId == account.Id))
            {
                account.FollowerDestinationAccounts.Add(follower);
            }

            return account.ToDTO();
        }
    }
}
