using System.Collections.Generic;

namespace MyBlog.Domain.Core
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string AboutMy { get; set; }
        public string Photo { get; set; }


        /// <summary>
        /// Навигационное свойство ролей
        /// </summary>
        public List<Role> Role { get; set; } = new List<Role>();
        /// <summary>
        /// Навигационное свойство статей
        /// </summary>
        public List<Article> Article { get; set; } = new List<Article>();
        /// <summary>
        /// Навигационное свойство комментариев
        /// </summary>
        public List<Comment> Comment { get; set; } = new List<Comment>();        
    }
}
