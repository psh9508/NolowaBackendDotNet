using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using SharedLib.Dynamodb.Models;
using SharedLib.Dynamodb.Service;
using SharedLib.Messages;
using SharedLib.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAccountsService
    {
        Task<Models.DTOs.DdbUser> FindAsync(long id);
        //Task<AccountDTO> LoginAsync(string email, string password);
        Task<LoginRes> LoginAsync(string email, string password);
        Task<SharedLib.Models.User> SaveAsync(IFSignUpUser newAccount);
        Task<bool> HasFollowedAsync(IFFollowModel data);
        Task<FollowerDTO> FollowAsync(IFFollowModel data);
        Task<FollowerDTO> UnFollowAsync(IFFollowModel data);
        Task<bool> ChangeProfileInfoAsync(ProfileInfoDTO data);
    }

    public class AccountsService : ServiceBase<AccountsService>, IAccountsService
    {
        private readonly IDbService _ddbService;

        public AccountsService(IJWTTokenProvider jwtTokenProvider, IDbService ddbService) : base(jwtTokenProvider)
        {
            _ddbService = ddbService;
        }

        public async Task<Models.DTOs.DdbUser> FindAsync(long id)
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
            var user = await _ddbService.FindAsync<SharedLib.Dynamodb.Models.DdbUser>($"u#{email}");
            
            if (user == null)
                return null;

            if(password.ToSha256() != user.Password)
            {
                // 비밀번호 불일치
                return null;
            }

            var loginRes = new LoginRes()
            {
                //Id = accountDTO.Id,
                Id = long.Parse(user.USN),
                UserId = user.UserId,
                AccountName = user.AccountName,
                Email = user.Email,
            };

            loginRes.Jwt = _jwtTokenProvider.GenerateJWTToken(user);

            return loginRes;
        }

        public async Task<SharedLib.Models.User> SaveAsync(IFSignUpUser signUpUserIFModel)
        {
            #region legacy
            //using var transaction = _context.Database.BeginTransaction();

            //try
            //{
            //    var beInsertedUser = _mapper.Map<Account>(signUpUserIFModel);

            //    // The Password must be encoded by SHA256
            //    beInsertedUser.Password = beInsertedUser.Password?.ToSha256();
            //    beInsertedUser.UserId = "RandomID";

            //    if (HasProfileImage(signUpUserIFModel))
            //    {
            //        var savedGuid = await SaveProfileImageFileToPhysicalLayer(signUpUserIFModel.ProfileImage);

            //        beInsertedUser.ProfileInfo = new ProfileInfo()
            //        {
            //            ProfileImg = new ProfileImage()
            //            {
            //                FileHash = savedGuid.ToString(),
            //                Url = "newUrl",
            //            },
            //        };
            //    }

            //    _context.Accounts.Add(beInsertedUser);
            //    await _context.SaveChangesAsync();

            //    transaction.Commit();

            //    var savedAccount = await _context.Accounts.Where(x => x.Id == beInsertedUser.Id).SingleAsync();

            //    return _mapper.Map<AccountDTO>(savedAccount);
            //}
            //catch (Exception ex)
            //{
            //    transaction.Rollback();
            //    return null;
            //}
            #endregion

            try
            {
                var saveModel = new SharedLib.Dynamodb.Models.DdbUser();
                //saveModel.Id = 1;
                saveModel.USN = "1";
                saveModel.AccountName = signUpUserIFModel.AccountName;
                saveModel.Email = signUpUserIFModel.Email;
                saveModel.Password = signUpUserIFModel.Password.ToSha256();
                saveModel.JoinDate = DateTime.Now;

                var savedData = await _ddbService.SaveAsync(saveModel);

                // 이메일로 비밀번호 가져올 수 있는 접근패턴에 대응하기 위한 데이터
                saveModel.Key = saveModel.Email;
                await _ddbService.SaveAsync(saveModel);

                return new SharedLib.Models.User()
                {
                    USN = savedData.USN,
                    UserId = savedData.UserId,
                    AccountName = saveModel.AccountName,
                    Email = saveModel.Email,
                    JoinDate = saveModel.JoinDate,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<Models.DTOs.DdbUser> FindAsync(Expression<Func<Account, bool>> whereExpression)
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

                var accountDTO = _mapper.Map<Models.DTOs.DdbUser>(account);

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
