using System;

namespace web.professionaltranslator.net.Areas.Blog.Models.Forms
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public string Categories { get; set; }
        public string Slug { get; set; }
        public DateTime? PubDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
