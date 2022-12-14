using AutoMapper;
using MiBlog.Api.Contracts.Models.Comments;
using MiBlog.Api.Contracts.Models.Roles;
using MiBlog.Api.Contracts.Models.Tags;
using MiBlog.Api.Contracts.Models.Users;
using MyBlog.Domain.Core;
using MyBlog.Infrastructure.Business.Models;

namespace MyBlog.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserLoginResponse>();
            CreateMap<UserRegisterRequest, User>().ForMember(x => x.Password, opt => opt.MapFrom(src => src.RegPassword));

            CreateMap<Article, ArticleModel>();
            CreateMap<User, UserModel>();
            CreateMap<Comment, CommentModel>();
            CreateMap<Tag, TagModel>();
            CreateMap<TagModel, Tag>();

            CreateMap<Comment, CommentResponse>();

            CreateMap<Tag, TagResponse>();

            CreateMap<Role, RoleResponse>();

            CreateMap<User, UserResponse>();
        }
    }
}
