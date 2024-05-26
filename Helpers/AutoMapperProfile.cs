using API.DTOs;
using API.Entities;
using API.Extension;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, UserRegisterDTO>().ReverseMap();
            CreateMap<AppUser,MemberDto>().ForMember(des=>des.Url,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain ).Url))
                .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalCulateAge()));
            CreateMap<Photo,PhotoDto>();
        }
    }
}