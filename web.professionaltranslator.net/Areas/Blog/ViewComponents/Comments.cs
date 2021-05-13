using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web.professionaltranslator.net.Areas.Blog.Models;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;

namespace web.professionaltranslator.net.Areas.Blog.ViewComponents
{
    public class Comments : ViewComponent
    {
        internal readonly IBlogService BlogService;

        public Comments(IBlogService blogService)
        {
            BlogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postId = Session.Get<string>(HttpContext.Session, Session.Key.PostId);
            var showComments = Session.Get<bool>(HttpContext.Session, Session.Key.ShowComments);
            var commentsAreOpen = Session.Get<bool>(HttpContext.Session, Session.Key.CommentsAreOpen);
            var userAuthenticated = Session.Get<bool>(HttpContext.Session, Session.Key.UserAuthenticated);

            List<Comment> comments = await BlogService.GetComments(postId).ToListAsync();
            var model = new Models.Components.Comments
            {
                ShowComments = showComments,
                CommentsAreOpen = commentsAreOpen,
                UserAuthenticated = userAuthenticated,
                BeFirstToComment = !comments.Any(),
                List = comments
            };
            return View(model);
        }
    }
}
