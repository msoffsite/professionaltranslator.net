using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;
using WebEssentials.AspNetCore.Pwa;
using DataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;

using DirectoryComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class PostsModel : Base
    {
        internal readonly WebManifest Manifest;

        public PostsModel(SiteSettings siteSettings, WebManifest webManifest)
        {
            SiteSettings = siteSettings;
            Manifest = webManifest;
        }

        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> OnGet()
        {
            var directoryModel = new DirectoryComponentModel
            {
                AspPage = "/Posts",
                Category = string.Empty,
                Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            };

            Session.Json.SetObject(HttpContext.Session, Session.Key.DirectoryComponentModel, directoryModel);

            ViewData[Constants.Title] = Manifest.Name;
            ViewData[Constants.Description] = Manifest.Description;

            Item = await new Base().Get(SiteSettings, Blog, "BlogPosts");
            return Item == null ? NotFound() : (IActionResult)Page();
        }

        /*
        public async Task<IActionResult> GetCategory(string category, int page = 0)
        {
            // get posts for the selected category.
            var posts = this.blog.GetPostsByCategory(category);

            // apply paging filter.
            var filteredPosts = posts.Skip(this.settings.Value.PostsPerPage * page).Take(this.settings.Value.PostsPerPage);

            // set the view option
            this.ViewData["ViewOption"] = this.settings.Value.ListView;

            this.ViewData[Constants.TotalPostCount] = await posts.CountAsync().ConfigureAwait(true);
            this.ViewData[Constants.Title] = $"{this.manifest.Name} {category}";
            this.ViewData[Constants.Description] = $"Articles posted in the {category} category";
            this.ViewData[Constants.prev] = $"/blog/category/{category}/{page + 1}/";
            this.ViewData[Constants.next] = $"/blog/category/{category}/{(page <= 1 ? null : page - 1 + "/")}";
            return this.View("~/Views/Blog/Index.cshtml", filteredPosts.AsAsyncEnumerable());
        }
        */
    }
}
