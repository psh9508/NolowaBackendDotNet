using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using SharedLib.Messages;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAccountsService
    {
        Task<AccountDTO> FindAsync(long id);
        //Task<AccountDTO> LoginAsync(string email, string password);
        Task<LoginRes> LoginAsync(string email, string password);
        Task<AccountDTO> SaveAsync(IFSignUpUser newAccount);
        Task<bool> HasFollowedAsync(IFFollowModel data);
        Task<FollowerDTO> FollowAsync(IFFollowModel data);
        Task<FollowerDTO> UnFollowAsync(IFFollowModel data);
        Task<bool> ChangeProfileInfoAsync(ProfileInfoDTO data);
    }

    public class AccountsService : ServiceBase<AccountsService>, IAccountsService
    {
        private readonly IJWTTokenProvider _jwtTokenProvider;

        public AccountsService(IJWTTokenProvider jwtTokenProvider)
        {
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<AccountDTO> FindAsync(long id)
        {
            return await FindAsync(x => x.Id == id);
        }

        //public async Task<AccountDTO> LoginAsync(string email, string password)
        //{
        //    var accountDTO = await FindAsync(x => x.Email == email && x.Password == password.ToSha256());

        //    if (accountDTO == null)
        //        return null;

        //    accountDTO.JWTToken = _jwtTokenProvider.GenerateJWTToken(accountDTO);

        //    return accountDTO;
        //}

        public async Task<LoginRes> LoginAsync(string email, string password)
        {
            //var accountDTO = await FindAsync(x => x.Email == email && x.Password == password.ToSha256());
            
            // 테스트 동안은 실제로 DB를 가지 않고 리턴한다.
            var accountDTO = new AccountDTO()
            {
                Id = 1,
                UserId = "Noname",
                AccountName = "AccountName",
                Email = "Noname@domain.com",
            };

            if (accountDTO == null)
                return null;

            var loginRes = new LoginRes()
            {
                Id = accountDTO.Id,
                UserId = accountDTO.UserId,
                AccountName = accountDTO.AccountName,
                Email = accountDTO.Email,
            };

            loginRes.Jwt = _jwtTokenProvider.GenerateJWTToken(accountDTO);

            return loginRes;
        }

        public async Task<AccountDTO> SaveAsync(IFSignUpUser signUpUserIFModel)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var beInsertedUser = _mapper.Map<Account>(signUpUserIFModel);

                // The Password must be encoded by SHA256
                beInsertedUser.Password = beInsertedUser.Password?.ToSha256();
                beInsertedUser.UserId = "RandomID";

                if (HasProfileImage(signUpUserIFModel))
                {
                    var savedGuid = await SaveProfileImageFileToPhysicalLayer(signUpUserIFModel.ProfileImage);

                    beInsertedUser.ProfileInfo = new ProfileInfo()
                    {
                        ProfileImg = new ProfileImage()
                        {
                            FileHash = savedGuid.ToString(),
                            Url = "newUrl",
                        },
                    };
                }

                _context.Accounts.Add(beInsertedUser);
                await _context.SaveChangesAsync();

                transaction.Commit();

                var savedAccount = await _context.Accounts.Where(x => x.Id == beInsertedUser.Id).SingleAsync();

                return _mapper.Map<AccountDTO>(savedAccount);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return null;
            }
        }

        private async Task<AccountDTO> FindAsync(Expression<Func<Account, bool>> whereExpression)
        {
            using(var context = new NolowaContext())
            {
                var account = await context.Accounts.Where(whereExpression)
                                               .Include(account => account.FollowerSourceAccounts)
                                               .Include(account => account.ProfileInfo)
                                               .ThenInclude(profileInfo => profileInfo.ProfileImg)
                                               .FirstOrDefaultAsync();

                if (account == null)
                    return null;

                var accountDTO = _mapper.Map<AccountDTO>(account);

                // 본인 아이디가 키인 데이터를 가져와서 그 데이터에 DestinationID로 Follower의 Post를 가져와야한다. 그래서 SourceAccount를 가져와 반복문을 도는 것임.
                foreach (var follower in account.FollowerSourceAccounts)
                {
                    var followerPost = context.Posts.Where(x => x.AccountId == follower.DestinationAccountId)
                                                     .Include(x => x.Account)
                                                     .ThenInclude(x => x.ProfileInfo)
                                                     .ThenInclude(profileInfo => profileInfo.ProfileImg)
                                                     .OrderByDescending(x => x.InsertDate)
                                                     .Select(x => _mapper.Map<PostDTO>(x))
                                                     .Take(10);

                    accountDTO.Posts.ToList().AddRnage(followerPost);
                }

                return accountDTO;
            }
        }

        public async Task<bool> HasFollowedAsync(IFFollowModel data)
        {
            return await _context.Followers.AnyAsync(x => x.SourceAccountId == data.SourceID && x.DestinationAccountId == data.DestID);
        }

        public async Task<FollowerDTO> FollowAsync(IFFollowModel data)
        {
            try
            {
                var newFollowRow = new Follower()
                {
                    SourceAccountId = data.SourceID,
                    DestinationAccountId = data.DestID,
                };

                await _context.Followers.AddAsync(newFollowRow);
                await _context.SaveChangesAsync();

                var savedData = await _context.Followers.FirstAsync(x => x.Id == newFollowRow.Id);

                return _mapper.Map<FollowerDTO>(savedData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FollowerDTO> UnFollowAsync(IFFollowModel data)
        {
            try
            {
                var unfollowData = await _context.Followers.FirstOrDefaultAsync(x => x.SourceAccountId == data.SourceID && x.DestinationAccountId == data.DestID);

                if (unfollowData.IsNull())
                    throw new InvalidOperationException("삭제할 팔로우 상태가 없습니다.");

                _context.Followers.Remove(unfollowData);
                var changedRowNum = await _context.SaveChangesAsync();

                if (changedRowNum <= 0)
                    return null;

                return _mapper.Map<FollowerDTO>(unfollowData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ChangeProfileInfoAsync(ProfileInfoDTO data)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var beUpdatedData = await _context.ProfileInfos.SingleAsync(x => x.Id == data.Id);

                // 이렇게 하는게 맞는건가? //
                // FK가 변경 된다. //
                beUpdatedData.ProfileImg = _mapper.Map<ProfileImage>(data.ProfileImage);
                beUpdatedData.BackgroundImg = _mapper.Map<ProfileImage>(data.BackgroundImage);
                beUpdatedData.Message = data.Message;

                // 이렇게 하는게 맞는가? //
                //var test = _mapper.Map<ProfileInfo>(data);
                //beUpdatedData = test;

                await _context.SaveChangesAsync();

                transaction.Commit();
                
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return false;
            }
        }

        private static bool HasProfileImage(IFSignUpUser signUpUserIFModel)
        {
            return signUpUserIFModel.ProfileImage?.Length > 0;

        }

        private static async Task<Guid> SaveProfileImageFileToPhysicalLayer(byte[] profileImage)
        {
            // Hash로 파일명을 계산하니 파일명으로 사용할 수 없는 문자도 나와서 임시로 변경
            //var saveFullPath = Constant.PROFILE_IMAGE_ROOT_PATH + @$"{profileImage.ToSha256()}.jpg";
            var guid = Guid.NewGuid();
            var saveFullPath = Constant.PROFILE_IMAGE_ROOT_PATH + @$"{guid}.jpg";

            using (var filestream = System.IO.File.Create(saveFullPath))
            {
                await filestream.WriteAsync(profileImage, 0, profileImage.Length);
                filestream.Flush();
            }

            return guid;
        }
    }
}
