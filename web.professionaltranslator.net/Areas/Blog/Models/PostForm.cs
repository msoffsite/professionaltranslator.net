using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.professionaltranslator.net.Areas.Blog.Models
{
    public class PostForm
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public string Categories { get; set; }
        public string Slug { get; set; }
        public bool IsPublished { get; set; }
    }
}
