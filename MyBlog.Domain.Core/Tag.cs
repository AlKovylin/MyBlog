using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Domain.Core
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Навигационное свойство статей
        /// </summary>
        public List<Article> Articles { get; set; } = new List<Article>();
    }
}
