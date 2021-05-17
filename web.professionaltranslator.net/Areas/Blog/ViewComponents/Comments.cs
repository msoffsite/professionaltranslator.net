using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web.professionaltranslator.net.Areas.Blog.Models;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;

using CommentsComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Comments;

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
            var model = Session.Json.GetObject<CommentsComponentModel>(HttpContext.Session, Session.Key.CommentsComponentModel);
            List<Comment> comments = await BlogService.GetComments(model.PostId).ToListAsync();
            model.List = comments;
            return View(model);
        }
    }
}
