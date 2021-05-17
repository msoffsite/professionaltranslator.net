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
    }
}
