using AutoMapper;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDTO>()
                .ForMember(d => d.ProfileImage, o => o.NullSubstitute(new ProfileImage()
                {
                    FileHash = "ProfilePicture", // default profile Image name
                }))
                .ForMember(d => d.Followers, o => o.MapFrom(s => s.FollowerSourceAccounts))
                .ReverseMap();

            CreateMap<Post, PostDTO>().ReverseMap();

            CreateMap<ProfileImage, ProfileImageDTO>()
                .ForMember(d => d.Hash, o => o.MapFrom(s => s.FileHash))
                .ReverseMap();

            CreateMap<Follower, FollowerDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.DestinationAccountId))
                .ReverseMap();
        }
    }
}
