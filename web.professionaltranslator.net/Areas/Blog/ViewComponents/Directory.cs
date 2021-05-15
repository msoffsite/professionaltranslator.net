using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using web.professionaltranslator.net.Areas.Blog.Models;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;
using DirectoryModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;

namespace web.professionaltranslator.net.Areas.Blog.ViewComponents
{
    public class Directory : ViewComponent
    {
        internal readonly IBlogService BlogService;
        internal readonly IOptionsSnapshot<BlogSettings> BlogSettings;

        public Directory(IOptionsSnapshot<BlogSettings> blogSettings, IBlogService blogService)
        {
            BlogSettings = blogSettings;
            BlogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = Session.Json.GetObject<DirectoryModel>(HttpContext.Session, Session.Key.DirectoryComponentModel);
            if (string.IsNullOrWhiteSpace(model.Category))
            {
                model.Data = await BlogService.GetPosts().ToListAsync();
            }
            else
            {
                model.Data = await BlogService.GetPostsByCategory(model.Category).ToListAsync();
            }

            int postsPerPage = BlogSettings.Value.PostsPerPage;
            int dataCount = model.Data.Count();

            if (dataCount <= postsPerPage)
            {
                model.PageCount = 1;
            }
            else
            {
                int pageCount = (dataCount + BlogSettings.Value.PostsPerPage - 1) / BlogSettings.Value.PostsPerPage;
                model.PageCount = pageCount;
            }

            int pageIndex = model.CurrentPage - 1;
            model.Data = model.Data.Skip(BlogSettings.Value.PostsPerPage * pageIndex).Take(BlogSettings.Value.PostsPerPage).ToList();

            model.Data = model.Data.OrderByDescending(p => p.PubDate).ToList();

            ViewData[Constants.ViewOption] = BlogSettings.Value.ListView;
            ViewData[Constants.Previous] = $"/{model.CurrentPage + 1}/";
            ViewData[Constants.Next] = $"/{(model.CurrentPage <= 1 ? null : $"{model.CurrentPage - 1}/")}";

            return View(model);
        }
    }
}
