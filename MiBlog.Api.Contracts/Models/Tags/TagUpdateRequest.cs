using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBlog.Api.Contracts.Models.Tags
{
    public class TagUpdateRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
