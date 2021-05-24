using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Extensions;
using CommentsComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Comments;
using DirectoryComponentModel = web.professionaltranslator.net.Areas.Blog.Models.Components.Directory;
using PostDataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;
using SubscriberDataModel = Models.ProfessionalTranslator.Net.Subscriber;
using SubscriberModel = web.professionaltranslator.net.Areas.Blog.Models.Subscriber;
using SubscriberRepository = Repository.ProfessionalTranslator.Net.Subscriber;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class IndexModel : Base
    {
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        public PostDataModel Data { get; set; }

        public bool HasCategories { get; set; }

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

            UserAuthenticated = User.Identity.IsAuthenticated;
            HasCategories = Data.Categories.Count > 0;
            ShowComments = BlogSettings.Value.DisplayComments;

            var commentComponentModel = new CommentsComponentModel
            {
                PostId = Data.Id,
                CommentsAreOpen = Data.AreCommentsOpen(BlogSettings.Value.CommentsCloseAfterDays),
                ShowComments = ShowComments,
                UserAuthenticated = UserAuthenticated
            };

            Session.Json.SetObject(HttpContext.Session, Session.Key.CommentsComponentModel, commentComponentModel);

            var directoryModel = new DirectoryComponentModel
            {
                AspPage = "/Index",
                Category = string.Empty,
                Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            };

            Session.Json.SetObject(HttpContext.Session, Session.Key.DirectoryComponentModel, directoryModel);

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
                body.Append($"Thank you for subscribing to my blog, {obj.FirstName}.");
                body.Append("<p>");
                body.Append("You can expect an email or two per week announcing my latest blog post(s). ");
                body.Append("Please feel free to comment whenever you feel the urge. ");
                body.Append("Also, please be aware subscribers will be the first to know about any giveaways I might offer in the future. ");
                body.Append("Until next time...");
                body.Append("</p>");
                body.Append("<p>");
                body.Append("Happy reading!");
                body.Append("</p>");
                body.Append("<p>&nbsp;</p>");
                body.Append("<p>");
                body.Append("Cinta Garcia, <a href=\"https://professionaltranslator.net\">Professional Translator</a>");
                body.Append("</p>");
                body.Append("<p>&nbsp;</p>");
                body.Append("<p>");
                body.Append("P.S. You can unsubscribe from my blog at any time by visiting <a href=\"https://professionaltranslator.net/unsubscribe\">professionaltranslator.net/unsubscribe</a>.");
                body.Append("</p>");
                
                var toList = new List<MailAddress>
                {
                    new MailAddress(obj.EmailAddress, fullName)
                };

                var ccList = new List<MailAddress>
                {
                    new MailAddress(SiteSettings.DefaultTo, SiteSettings.DefaultToDisplayName)
                };

                var replyToList = new List<MailAddress>
                {
                    new MailAddress(obj.EmailAddress, fullName)
                };

                Smtp.SendMail(SiteSettings, replyToList, toList, ccList, new List<MailAddress>(), 
                    $"Welcome, {fullName}!", body.ToString(), Smtp.BodyType.Html, Smtp.SslSetting.Off);
            }
            catch (System.Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }
    }
}
