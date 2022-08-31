using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Domain.Core
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Навигационное свойство тегов
        /// </summary>
        public List<User> User { get; set; } = new List<User>();
    }
}
