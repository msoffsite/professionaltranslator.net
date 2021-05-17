using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using web.professionaltranslator.net.Extensions;
using WebEssentials.AspNetCore.Pwa;
using DirectoryComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class PostsModel : Base
    {
        //internal readonly WebManifest Manifest;

        //public PostsModel(SiteSettings siteSettings, WebManifest webManifest)
        //{
        //    SiteSettings = siteSettings;
        //    Manifest = webManifest;
        //}

        public PostsModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
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

            //ViewData[Constants.Title] = Manifest.Name;
            //ViewData[Constants.Description] = Manifest.Description;

            ViewData[Constants.Title] = "Cinta Garcia - Professional Translator";
            ViewData[Constants.Description] = "Blog for Cinta Garica, Professional Translator. English to Spanish and vice versa.";

            Item = await new Base().Get(SiteSettings, Blog, "BlogPosts");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
