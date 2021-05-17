using System.Collections.Generic;

namespace web.professionaltranslator.net.Areas.Blog.Models.Components
{
    public class Directory
    {
        public string AspPage { get; set; }
        public string Category { get; set; }
        public int CurrentPage { get; set; } = 1;
        public List<Post> Data { get; set; }
        public string Host { get; set; }
        public int PageCount { get; set; }
    }
}
