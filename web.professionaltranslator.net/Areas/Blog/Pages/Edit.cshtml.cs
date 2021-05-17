#nullable enable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using web.professionaltranslator.net.Areas.Blog.Models;
using web.professionaltranslator.net.Areas.Blog.Services;
using WebEssentials.AspNetCore.Pwa;
using DataModel = web.professionaltranslator.net.Areas.Blog.Models.Post;
using Exception = System.Exception;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : Base
    {
        internal readonly WebManifest Manifest;

        [BindProperty(SupportsGet = true)]
        public string? PostId { get; set; } = string.Empty;

        public DataModel? Data { get; set; }

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

        public async Task<IActionResult> OnPostSave()
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

                var obj = JsonConvert.DeserializeObject<Models.Forms.Post>(requestBody);
                if (obj == null) throw new NullReferenceException("Model could not be derived from JSON object.");

                Data = await BlogService.GetPostById(obj.Id).ConfigureAwait(false) ?? new DataModel
                {
                    Id = obj.Id,
                    PubDate = DateTime.Now
                };

                Data.Content = obj.Content.Trim();
                Data.Excerpt = obj.Excerpt.Trim();
                Data.IsPublished = obj.IsPublished;
                Data.LastModified = DateTime.Now;
                Data.Slug = obj.Slug.Trim();
                Data.Title = obj.Title.Trim();

                Data.Categories.Clear();
                obj.Categories.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim().ToLowerInvariant())
                    .ToList()
                    .ForEach(Data.Categories.Add);

                await SaveFilesToDisk(Data).ConfigureAwait(false);

                await BlogService.SavePost(Data).ConfigureAwait(false);

                result = new Result(ResultStatus.Succeeded, Data.GetEncodedLink(), null);
            }
            catch (Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }

        private async Task SaveFilesToDisk(Post post)
        {
            var imgRegex = new Regex("<img[^>]+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);
            var allowedExtensions = new[] {
              ".jpg",
              ".jpeg",
              ".gif",
              ".png",
              ".webp"
            };

            foreach (Match? match in imgRegex.Matches(post.Content))
            {
                if (match is null)
                {
                    continue;
                }

                var doc = new XmlDocument();
                doc.LoadXml($"<root>{match.Value}</root>");

                XmlNode img = doc.FirstChild.FirstChild;
                if (img?.Attributes == null)
                {
                    throw new NullReferenceException("XmlNode for image wasn't found.");
                }

                XmlAttribute srcNode = img.Attributes["src"];
                XmlAttribute fileNameNode = img.Attributes["data-filename"];

                // The HTML editor creates base64 DataURIs which we'll have to convert to image
                // files on disk
                if (srcNode is null || fileNameNode is null)
                {
                    continue;
                }

                string extension = Path.GetExtension(fileNameNode.Value);

                // Only accept image files
                if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                Match base64Match = base64Regex.Match(srcNode.Value);

                if (!base64Match.Success)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(fileNameNode?.Value))
                {
                    throw new NullReferenceException("fileNameNode is missing.");
                }

                byte[] bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                srcNode.Value = await BlogService.SaveFile(bytes, fileNameNode.Value).ConfigureAwait(false);

                img.Attributes.Remove(fileNameNode);
                post.Content = post.Content.Replace(match.Value, img.OuterXml, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
