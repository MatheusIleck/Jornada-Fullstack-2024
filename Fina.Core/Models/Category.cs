using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fina.Core.Models
{
    internal class Category
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Descriptions { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
