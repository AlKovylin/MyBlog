using MyBlog.Infrastructure.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class ArticleCreateViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Display(Name = "Название статьи", Prompt = "Введите название статьи")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Display(Name = "Содержимое статьи", Prompt = "Введите текст")]
        public string Content { get; set; }
        public DateTime Published { get; set; } = DateTime.Now;
        public List<TagModel> Tags = new List<TagModel>();
        public List<TagModel> AllTags = new List<TagModel>();
    }
}
