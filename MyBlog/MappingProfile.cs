using AutoMapper;
using MyBlog.Domain.Core;
using MyBlog.Infrastructure.Business.Models;
using MyBlog.ViewModels;
using System.Linq;

namespace MyBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserModel, User>();
            CreateMap<User, UserModel>();

            CreateMap<UserViewModel, User>()
                .ForMember(u => u.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<User, UserViewModel>()
                .ForMember(u => u.FirstName, opt => opt.MapFrom(src => src.Name.Split(new char[] { ' ' }).First()))
                .ForMember(u => u.LastName, opt => opt.MapFrom(src => src.Name.Split(new char[] { ' ' }).Last()));

            CreateMap<ArticleModel, Article>();
            CreateMap<Article, ArticleModel>();

            CreateMap<ArticleEditViewModel, Article>();
            CreateMap<Article, ArticleEditViewModel>();

            CreateMap<CommentModel, Comment>();
            CreateMap<Comment, CommentModel>();

            CreateMap<TagModel, Tag>();
            CreateMap<Tag, TagModel>();

            CreateMap<TagsViewModel, Tag>();
        }
    }
}
