using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAccountsService
    {
        Task<AccountDTO> FindAsync(long id);
        AccountDTO Login(string email, string password);
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
            var account = await _context.Accounts.FindAsync(id);

            return account == null ? null : account.ToDTO();
        }

        public AccountDTO Login(string email, string password)
        {
            var account = _context.Accounts.Where(x => x.Email == email && x.Password == password)
                                    .Include(account => account.FollowerDestinationAccounts)
                                    .Include(account => account.FollowerSourceAccounts)
                                    .Include(account => account.Posts.OrderByDescending(x => x.InsertDate).Take(10))
                                    .Include(account => account.ProfileImage)
                                    .FirstOrDefault();

            if (account == null)
                return null;

            var accountDTO = account.ToDTO();

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
    }
}
