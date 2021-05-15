using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using DirectoryComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class Base : net.Pages.Base
    {
        internal Area Blog = Area.Blog;

        public IActionResult OnPostShowComments()
        {
            return ViewComponent("Comments");
        }

        public IActionResult OnPostShowDirectory()
        {
            return ViewComponent("Directory");
        }

        public IActionResult OnPostFilterDirectory(string category)
        {
            try
            {
                var directoryComponentModel =
                    Session.Json.GetObject<DirectoryComponentModel>(HttpContext.Session, Session.Key.DirectoryComponentModel);
                directoryComponentModel.CurrentPage = 1;
                directoryComponentModel.Category = category;
                Session.Json.SetObject(HttpContext.Session, Session.Key.DirectoryComponentModel, directoryComponentModel);
            }
            catch (NullReferenceException)
            {
                return Redirect("/Blog/Posts");
            }

            return ViewComponent("Directory");
        }

        public IActionResult OnPostShowDirectoryNextPage()
        {
            try
            {
                var directoryComponentModel =
                    Session.Json.GetObject<DirectoryComponentModel>(HttpContext.Session, Session.Key.DirectoryComponentModel);
                directoryComponentModel.CurrentPage -= 1;
                Session.Json.SetObject(HttpContext.Session, Session.Key.DirectoryComponentModel, directoryComponentModel);
            }
            catch (NullReferenceException)
            {
                return Redirect("/Blog/Posts");
            }
            
            return ViewComponent("Directory");
        }

        public IActionResult OnPostShowDirectoryPreviousPage()
        {
            try
            {
                var directoryComponentModel =
                    Session.Json.GetObject<DirectoryComponentModel>(HttpContext.Session, Session.Key.DirectoryComponentModel);
                directoryComponentModel.CurrentPage += 1;
                Session.Json.SetObject(HttpContext.Session, Session.Key.DirectoryComponentModel, directoryComponentModel);
            }
            catch (NullReferenceException)
            {
                return Redirect("/Blog/Posts");
            }
            
            return ViewComponent("Directory");
        }

        public async Task<IActionResult> OnPostSaveComment()
        {
            return null;
        }
    }
}