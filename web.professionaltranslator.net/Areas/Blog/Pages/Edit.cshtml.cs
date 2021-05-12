using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using web.professionaltranslator.net.Areas.Blog.Services;
using WebEssentials.AspNetCore.Pwa;

using DataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : Base
    {
        internal readonly IBlogService BlogService;
        internal readonly IOptionsSnapshot<BlogSettings> BlogSettings;
        internal readonly WebManifest Manifest;

        [BindProperty(SupportsGet = true)]
        public string PostId { get; set; } = string.Empty;

        public DataModel Data { get; set; }

        public EditModel(SiteSettings siteSettings, IOptionsSnapshot<BlogSettings> blogSettings, IBlogService blogService, WebManifest webManifest)
        {
            SiteSettings = siteSettings;
            BlogSettings = blogSettings;
            BlogService = blogService;
            Manifest = webManifest;
        }

        public async Task<IActionResult> OnGet()
        {
            List<string> categories = await BlogService.GetCategories().ToListAsync();
            categories.Sort();
            ViewData[Constants.AllCategories] = categories;

            if (string.IsNullOrWhiteSpace(PostId))
            {
                Data = new DataModel();
            }
            else
            {
                Data = await BlogService.GetPostById(PostId).ConfigureAwait(false);
            }

            Item = await new Base().Get(SiteSettings, Blog, "BlogEditPost");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
