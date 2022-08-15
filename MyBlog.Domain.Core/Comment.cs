using System;

namespace MyBlog.Domain.Core
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } =  DateTime.Now;

        /// <summary>
        /// Внешний ключ
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Внешний ключ
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public Article Article { get; set; }
    }
}
