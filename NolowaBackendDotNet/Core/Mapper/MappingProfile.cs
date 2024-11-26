using AutoMapper;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.IF;

namespace NolowaBackendDotNet.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Account, DdbUser>()
            //    .ForMember(d => d.ProfileInfo, o => o.NullSubstitute(new ProfileInfo()
            //    {
            //        ProfileImg = new ProfileImage()
            //        {
            //            FileHash = "ProfilePicture", // default profile Image name
            //        }
            //    }))
            //    .ForMember(d => d.Followers, o => o.MapFrom(s => s.FollowerSourceAccounts))
            //    .ReverseMap();

            //CreateMap<ProfileInfo, ProfileInfoDTO>()
            //    .ForMember(d => d.ProfileImage, o => o.NullSubstitute(new ProfileImage()
            //    {
            //        FileHash = "ProfilePicture", // default profile Image name
            //    }))
            //    .ForMember(d => d.ProfileImage, o => o.MapFrom(s => s.ProfileImg))
            //    .ForMember(d => d.BackgroundImage, o => o.MapFrom(s => s.BackgroundImg))
            //    .ReverseMap();

            //CreateMap<Post, PostDTO>().ReverseMap();

            //CreateMap<ProfileImage, ProfileImageDTO>()
            //    .ForMember(d => d.Hash, o => o.MapFrom(s => s.FileHash))
            //    .ReverseMap();

            //CreateMap<Follower, FollowerDTO>()
            //    .ForMember(d => d.Id, o => o.MapFrom(s => s.DestinationAccountId))
            //    .ReverseMap();

            #region IF
            CreateMap<IFSignUpUser, Account>();
            #endregion
        }
    }
}
