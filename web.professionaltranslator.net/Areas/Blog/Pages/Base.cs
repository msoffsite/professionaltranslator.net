using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using Repository.ProfessionalTranslator.Net.Conversions;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;

using CommentModel = web.professionaltranslator.net.Areas.Blog.Models.Comment;
using CommentsComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Comments;
using DirectoryComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;
using PostDataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;


namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class Base : net.Pages.Base
    {
        internal Area Blog = Area.Blog;

        internal IBlogService BlogService;

        internal IOptionsSnapshot<BlogSettings> BlogSettings;

        //ViewComponent Methods
        public async Task<IActionResult> OnPostSaveComment(string author, string email, string text, string exists)
        {
            if ((string.IsNullOrWhiteSpace(author)) || (string.IsNullOrWhiteSpace(email)) ||
                (string.IsNullOrWhiteSpace(text)) ||
                (string.IsNullOrWhiteSpace(exists))) return ViewComponent("Comments");
            
            var commentsComponentModel =
                Session.Json.GetObject<CommentsComponentModel>(HttpContext.Session, Session.Key.CommentsComponentModel);

            PostDataModel post = await BlogService.GetPostById(commentsComponentModel.PostId).ConfigureAwait(true);
            if (post == null) throw new NullReferenceException("Post could not be derived from session.");

            var commentModel = new CommentModel
            {
                Author = author,
                Email = email,
                IsAdmin = User.Identity.IsAuthenticated,
                Text = text
            };

            bool elementExists = Implicit.Bool(exists);
            // the website form key should have been removed by javascript unless the comment was posted by a spam robot
            // ReSharper disable once InvertIf
            if (!elementExists)
            {
                post.Comments.Add(commentModel);
                await BlogService.SavePost(post).ConfigureAwait(false);
                Session.Json.SetObject(HttpContext.Session, Session.Key.CommentsComponentModel, commentsComponentModel);
            }

            return ViewComponent("Comments");
        }
        public async Task<IActionResult> OnPostShowComments()
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
    }
}