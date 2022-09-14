using AutoMapper;
using MiBlog.Api.Contracts.Models.Users;
using MyBlog.Domain.Core;

namespace MyBlog.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserLoginResponse>();

            CreateMap<UserRegisterRequest, User>().ForMember(x => x.Password, opt => opt.MapFrom(src => src.RegPassword));
        }
    }
}
