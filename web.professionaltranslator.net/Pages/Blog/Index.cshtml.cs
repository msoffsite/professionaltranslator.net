using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Services;
using DataModel = web.professionaltranslator.net.Models.Post;

namespace web.professionaltranslator.net.Pages.Blog
{
    public class IndexModel : Base
    {
        internal readonly IBlogService BlogService;

        internal readonly IOptionsSnapshot<BlogSettings> BlogSettings;

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        public DataModel Data { get; set; }

        public bool HasCategories { get; set; }

        public bool CommentsAreOpen { get; set; }

        public bool BeFirstToComment { get; set; }

        public bool ShowComments { get; set; }

        public bool UserAuthenticated { get; set; }
        

        public IndexModel(SiteSettings siteSettings, IBlogService blogService, IOptionsSnapshot<BlogSettings> blogSettings)
        {
            SiteSettings = siteSettings;
            BlogService = blogService;
            BlogSettings = blogSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Data = await BlogService.GetPostBySlug(Slug);
            if (Data == null)
            {
                string notFound = "No post found for " + Slug + ".";

                Data = new DataModel
                {
                    Content = "&lt;p&gt;" + notFound + "&lt;/p&gt;",
                    Excerpt = notFound,
                    Title = "Not Found",
                    Categories = { },
                    IsPublished = true,
                    LastModified = DateTime.Now,
                    PubDate = DateTime.Now,
                    Slug = this.Slug
                };
            }

            UserAuthenticated = User.Identity.IsAuthenticated;
            HasCategories = Data.Categories.Count > 0;
            ShowComments = BlogSettings.Value.DisplayComments && Data.Comments.Any();
            CommentsAreOpen = Data.AreCommentsOpen(BlogSettings.Value.CommentsCloseAfterDays);
            BeFirstToComment = Data.Comments.Count == 0;

            Item = await new Base().Get(SiteSettings, Area.Root, "BlogEntry");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
