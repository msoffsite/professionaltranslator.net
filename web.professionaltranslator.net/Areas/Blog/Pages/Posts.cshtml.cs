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
        public int CurrentPage { get; set; } = 0;

        public List<DataModel> Data { get; set; }

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
            Data = await BlogService.GetPosts().Skip(BlogSettings.Value.PostsPerPage * CurrentPage).Take(BlogSettings.Value.PostsPerPage).ToListAsync();

            ViewData[Constants.ViewOption] = BlogSettings.Value.ListView;

            ViewData[Constants.TotalPostCount] = Data.Count();
            ViewData[Constants.Title] = Manifest.Name;
            ViewData[Constants.Description] = Manifest.Description;
            ViewData[Constants.Previous] = $"/{CurrentPage + 1}/";
            ViewData[Constants.Next] = $"/{(CurrentPage <= 1 ? null : $"{CurrentPage - 1}/")}";

            Item = await new Base().Get(SiteSettings, Area.Root, "BlogPosts");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
