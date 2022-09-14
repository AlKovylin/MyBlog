using AutoMapper;
using MiBlog.Api.Contracts.Models.Articles;
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

            //CreateMap<Article, ArticleResponse>();
        }
    }
}
