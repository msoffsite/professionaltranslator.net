using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;
using PostDataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;

using SubscriberModel = web.professionaltranslator.net.Areas.Blog.Models.Subscriber;
using SubscriberDataModel = Models.ProfessionalTranslator.Net.Subscriber;
using SubscriberRepository = Repository.ProfessionalTranslator.Net.Subscriber;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class IndexModel : Base
    {
        internal readonly IBlogService BlogService;

        internal readonly IOptionsSnapshot<BlogSettings> BlogSettings;

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        public PostDataModel Data { get; set; }

        public bool HasCategories { get; set; }

        public bool CommentsAreOpen { get; set; }

        public bool BeFirstToComment { get; set; }

        public bool ShowComments { get; set; }

        public bool UserAuthenticated { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = string.Empty;

        public List<PostDataModel> Directory { get; set; }

        public int PageCount { get; set; }

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

                Data = new PostDataModel
                {
                    Content = notFound,
                    Excerpt = notFound,
                    Title = "Not Found",
                    Categories = { },
                    IsPublished = true,
                    LastModified = DateTime.Now,
                    PubDate = DateTime.Now,
                    Slug = this.Slug
                };
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                Directory = await BlogService.GetPosts().ToListAsync();
            }
            else
            {
                Directory = await BlogService.GetPostsByCategory(Category).ToListAsync();
            }

            int postsPerPage = BlogSettings.Value.PostsPerPage;
            int dataCount = Directory.Count();

            if (dataCount <= postsPerPage)
            {
                PageCount = 1;
                CurrentPage = 1;
            }
            else
            {
                int pageCount = (dataCount + BlogSettings.Value.PostsPerPage - 1) / BlogSettings.Value.PostsPerPage;
                PageCount = pageCount;
            }

            int pageIndex = CurrentPage - 1;
            Directory = Directory.Skip(BlogSettings.Value.PostsPerPage * pageIndex).Take(BlogSettings.Value.PostsPerPage).ToList();

            Directory = Directory.OrderByDescending(p => p.PubDate).ToList();

            UserAuthenticated = User.Identity.IsAuthenticated;
            HasCategories = Data.Categories.Count > 0;
            ShowComments = BlogSettings.Value.DisplayComments && Data.Comments.Any();
            CommentsAreOpen = Data.AreCommentsOpen(BlogSettings.Value.CommentsCloseAfterDays);
            BeFirstToComment = Data.Comments.Count == 0;

            Item = await new Base().Get(SiteSettings, Blog, "BlogEntry");
            return Item == null ? NotFound() : (IActionResult)Page();
        }

        public async Task<ActionResult> OnPostSubscribe()
        {
            Result result;
            try
            {
                var stream = new MemoryStream();
                await Request.Body.CopyToAsync(stream);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                string requestBody = await reader.ReadToEndAsync();

                if (requestBody.Length <= 0) throw new IndexOutOfRangeException("requestBody is empty.");

                var obj = JsonConvert.DeserializeObject<SubscriberModel>(requestBody);
                if (obj == null) throw new NullReferenceException("Model could not be derived from JSON object.");

                var dataModel = new SubscriberDataModel
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = obj.EmailAddress,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName
                };

                result = await SubscriberRepository.Save(SiteSettings.Site, Area.Blog, dataModel);

                if (result.Status == ResultStatus.Succeeded)
                {
                    result.Messages = new List<string> { "You're subscribed!" };
                }

                string fullName = obj.FirstName + " " + obj.LastName;
                var body = new StringBuilder();
                body.Append("<b>Name:</b> " + fullName);
                body.Append("<br/>");
                body.Append("<b>Email Address:</b> " + obj.EmailAddress);
                body.Append("<br/>");

                var toList = new List<MailAddress>
                {
                    new MailAddress(SiteSettings.DefaultTo, SiteSettings.DefaultToDisplayName),
                    new MailAddress(obj.EmailAddress, fullName)
                };

                var replyToList = new List<MailAddress>
                {
                    new MailAddress(obj.EmailAddress, fullName)
                };

                Smtp.SendMail(SiteSettings, replyToList, toList, "New Blog Subscriber", body.ToString(), Smtp.BodyType.Html, Smtp.SslSetting.Off);
            }
            catch (System.Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }
    }
}
