using System;
using System.Collections.Generic;

namespace MyBlog.Domain.Core
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Published { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; }

        /// <summary>
        /// Внешний ключ
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Навигационное свойство комментариев
        /// </summary>
        public List<Comment> Comment { get; set; } = new List<Comment>();

        /// <summary>
        /// Навигационное свойство тегов
        /// </summary>
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
