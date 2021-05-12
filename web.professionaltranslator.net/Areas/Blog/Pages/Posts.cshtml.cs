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
using WebEssentials.AspNetCore.Pwa;
using DataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class PostsModel : Base
    {
        internal readonly IBlogService BlogService;
        internal readonly IOptionsSnapshot<BlogSettings> BlogSettings;
        internal readonly WebManifest Manifest;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = string.Empty;

        public List<DataModel> Data { get; set; }

        public int PageCount { get; set; }

        public PostsModel(SiteSettings siteSettings, IOptionsSnapshot<BlogSettings> blogSettings, IBlogService blogService, WebManifest webManifest)
        {
            SiteSettings = siteSettings;
            BlogSettings = blogSettings;
            BlogService = blogService;
            Manifest = webManifest;
        }

        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrWhiteSpace(Category))
            {
                Data = await BlogService.GetPosts().ToListAsync();
            }
            else
            {
                Data = await BlogService.GetPostsByCategory(Category).ToListAsync();
            }

            int postsPerPage = BlogSettings.Value.PostsPerPage;
            int dataCount = Data.Count();

            if (dataCount <= postsPerPage)
            {
                PageCount = 1;
            }
            else
            {
                int pageCount = (dataCount + BlogSettings.Value.PostsPerPage - 1) / BlogSettings.Value.PostsPerPage;
                PageCount = pageCount;
            }

            int pageIndex = CurrentPage - 1;
            Data = Data.Skip(BlogSettings.Value.PostsPerPage * pageIndex).Take(BlogSettings.Value.PostsPerPage).ToList();

            Data = Data.OrderByDescending(p => p.PubDate).ToList();

            ViewData[Constants.ViewOption] = BlogSettings.Value.ListView;

            ViewData[Constants.Title] = Manifest.Name;
            ViewData[Constants.Description] = Manifest.Description;
            ViewData[Constants.Previous] = $"/{CurrentPage + 1}/";
            ViewData[Constants.Next] = $"/{(CurrentPage <= 1 ? null : $"{CurrentPage - 1}/")}";

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
