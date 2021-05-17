using System.Collections.Generic;

namespace web.professionaltranslator.net.Areas.Blog.Models.Components
{
    public class Comments
    {
        public string PostId { get; set; }

        public bool CommentsAreOpen { get; set; }

        public bool BeFirstToComment { get; set; }

        public bool ShowComments { get; set; }

        public bool UserAuthenticated { get; set; }

        public List<Comment> List { get; set; }
    }
}
