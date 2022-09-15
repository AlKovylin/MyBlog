using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBlog.Api.Contracts.Models.Comments
{
    public class CommentCreateRequest
    {
        public int IdArticle { get; set; }
        public string? Content { get; set; }
    }
}
