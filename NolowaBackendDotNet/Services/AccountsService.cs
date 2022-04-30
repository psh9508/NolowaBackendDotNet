using AutoMapper;
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
        Task<AccountDTO> SaveAsync(Account newAccount);
    }

    public class AccountsService : IAccountsService
    {
        private readonly NolowaContext _context;
        private readonly IJWTTokenProvider _jwtTokenProvider;
        private readonly IMapper _mapper;

        public AccountsService(NolowaContext context, IJWTTokenProvider jwtTokenProvider, IMapper mapper)
        {
            _context = context;
            _jwtTokenProvider = jwtTokenProvider;
            _mapper = mapper;
        }

        public async Task<AccountDTO> FindAsync(long id)
        {
            return await FindAsync(x => x.Id == id);
        }

        public async Task<AccountDTO> LoginAsync(string email, string password)
        {
            var accountDTO = await FindAsync(x => x.Email == email && x.Password == password.ToSha256());

            if (accountDTO == null)
                return null;

            accountDTO.JWTToken = _jwtTokenProvider.GenerateJWTToken(accountDTO);

            return accountDTO;
        }

        public async Task<AccountDTO> SaveAsync(Account newAccount)
        {
            try
            {
                // The Password must be encoded by SHA256
                newAccount.Password = newAccount.Password.ToSha256();

                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                var savedAccount = await _context.Accounts.Where(x => x.Id == newAccount.Id).SingleAsync();

                return _mapper.Map<AccountDTO>(savedAccount);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<AccountDTO> FindAsync(Expression<Func<Account, bool>> whereExpression)
        {
            var account = await _context.Accounts.Where(whereExpression)
                                               .Include(account => account.FollowerSourceAccounts) 
                                               .Include(account => account.ProfileImage)
                                               .FirstOrDefaultAsync();
            if (account == null)
                return null;

            // 본인 아이디가 키인 데이터를 가져와서 그 데이터에 DestinationID로 Follower의 Post를 가져와야한다. 그래서 SourceAccount를 가져와 반복문을 도는 것임.
            foreach (var follower in account.FollowerSourceAccounts)
            {
                var followerPost = _context.Posts.Where(x => x.AccountId == follower.DestinationAccountId)
                                                 .Include(x => x.Account)
                                                 .ThenInclude(x => x.ProfileImage)
                                                 .OrderByDescending(x => x.InsertDate).Take(10);

                account.Posts.AddRnage(followerPost);
            }

            return _mapper.Map<AccountDTO>(account);
        }
    }
}
