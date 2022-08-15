using AutoMapper;
using MyBlog.Domain.Core;
using MyBlog.Infrastructure.Business.Models;
using MyBlog.ViewModels;

namespace MyBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserModel, User>();
            CreateMap<ArticleModel, Article>();
            CreateMap<CommentModel, Comment>();
            CreateMap<TagModel, Tag>();
            CreateMap<ArticleEditViewModel, Article>();
            CreateMap<Article, ArticleEditViewModel>();
            CreateMap<TagViewModel, Tag>();

            CreateMap<User, UserModel>();
            CreateMap<Article, ArticleModel>();
            CreateMap<Comment, CommentModel>();
            CreateMap<Tag, TagModel>();
        }
    }
}
