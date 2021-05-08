using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using web.professionaltranslator.net.Models;

namespace web.professionaltranslator.net.Services
{
    public class FileBlogService : IBlogService
    {
        private const string Files = "files";

        private const string Posts = "Posts";

        private readonly List<Post> _cache = new List<Post>();

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly string _folder;

        public FileBlogService(IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            _folder = Path.Combine(env.WebRootPath, Posts);
            _contextAccessor = contextAccessor;

            Initialize();
        }

        public Task DeletePost(Post post)
        {
            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            string filePath = GetFilePath(post);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (_cache.Contains(post))
            {
                _cache.Remove(post);
            }

            return Task.CompletedTask;
        }

        public virtual IAsyncEnumerable<string> GetCategories()
        {
            bool isAdmin = IsAdmin();

            return _cache
                .Where(p => p.IsPublished || isAdmin)
                .SelectMany(post => post.Categories)
                .Select(cat => cat.ToLowerInvariant())
                .Distinct()
                .ToAsyncEnumerable();
        }

        public virtual Task<Post?> GetPostById(string id)
        {
            bool isAdmin = IsAdmin();
            Post post = _cache.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(
                post is null || post.PubDate > DateTime.UtcNow || (!post.IsPublished && !isAdmin)
                ? null
                : post);
        }

        public virtual Task<Post?> GetPostBySlug(string slug)
        {
            bool isAdmin = IsAdmin();
            Post post = _cache.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(
                post is null || post.PubDate > DateTime.UtcNow || (!post.IsPublished && !isAdmin)
                ? null
                : post);
        }

        /// <remarks>Overload for getPosts method to retrieve all posts.</remarks>
        public virtual IAsyncEnumerable<Post> GetPosts()
        {
            bool isAdmin = IsAdmin();

            IAsyncEnumerable<Post> posts = _cache
                .Where(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
                .ToAsyncEnumerable();

            return posts;
        }

        public virtual IAsyncEnumerable<Post> GetPosts(int count, int skip = 0)
        {
            bool isAdmin = IsAdmin();

            IAsyncEnumerable<Post> posts = _cache
                .Where(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
                .Skip(skip)
                .Take(count)
                .ToAsyncEnumerable();

            return posts;
        }

        public virtual IAsyncEnumerable<Post> GetPostsByCategory(string category)
        {
            bool isAdmin = IsAdmin();

            IEnumerable<Post> posts = from p in _cache
                        where p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin)
                        where p.Categories.Contains(category, StringComparer.OrdinalIgnoreCase)
                        select p;

            return posts.ToAsyncEnumerable();
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            suffix = CleanFromInvalidChars(suffix ?? DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

            string? ext = Path.GetExtension(fileName);
            string name = CleanFromInvalidChars(Path.GetFileNameWithoutExtension(fileName));

            var fileNameWithSuffix = $"{name}_{suffix}{ext}";

            string absolute = Path.Combine(_folder, Files, fileNameWithSuffix);
            string? dir = Path.GetDirectoryName(absolute);

            Directory.CreateDirectory(dir);
            await using var writer = new FileStream(absolute, FileMode.CreateNew);
            await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

            return $"/{Posts}/{Files}/{fileNameWithSuffix}";
        }

        public async Task SavePost(Post post)
        {
            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            string filePath = GetFilePath(post);
            post.LastModified = DateTime.UtcNow;

            var doc = new XDocument(
                            new XElement("post",
                                new XElement("title", post.Title),
                                new XElement("slug", post.Slug),
                                new XElement("pubDate", FormatDateTime(post.PubDate)),
                                new XElement("lastModified", FormatDateTime(post.LastModified)),
                                new XElement("excerpt", post.Excerpt),
                                new XElement("content", post.Content),
                                new XElement("ispublished", post.IsPublished),
                                new XElement("categories", string.Empty),
                                new XElement("comments", string.Empty)
                            ));

            XElement categories = doc.XPathSelectElement("post/categories");
            foreach (string category in post.Categories)
            {
                categories.Add(new XElement("category", category));
            }

            XElement comments = doc.XPathSelectElement("post/comments");
            foreach (Comment comment in post.Comments)
            {
                comments.Add(
                    new XElement("comment",
                        new XElement("author", comment.Author),
                        new XElement("email", comment.Email),
                        new XElement("date", FormatDateTime(comment.PubDate)),
                        new XElement("content", comment.Content),
                        new XAttribute("isAdmin", comment.IsAdmin),
                        new XAttribute("id", comment.ID)
                    ));
            }

            await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
            }

            if (!_cache.Contains(post))
            {
                _cache.Add(post);
                SortCache();
            }
        }

        protected bool IsAdmin() => _contextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;

        protected void SortCache() => _cache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));

        private static string CleanFromInvalidChars(string input)
        {
            // ToDo: what we are doing here if we switch the blog from windows to unix system or
            // vice versa? we should remove all invalid chars for both systems

            string regexSearch = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            var r = new Regex($"[{regexSearch}]");
            return r.Replace(input, string.Empty);
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            const string UTC = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

            return dateTime.Kind == DateTimeKind.Utc
                ? dateTime.ToString(UTC, CultureInfo.InvariantCulture)
                : dateTime.ToUniversalTime().ToString(UTC, CultureInfo.InvariantCulture);
        }

        private static void LoadCategories(Post post, XElement doc)
        {
            XElement categories = doc.Element("categories");
            if (categories is null)
            {
                return;
            }

            post.Categories.Clear();
            categories.Elements("category").Select(node => node.Value).ToList().ForEach(post.Categories.Add);
        }

        private static void LoadComments(Post post, XElement doc)
        {
            XElement comments = doc.Element("comments");

            if (comments is null)
            {
                return;
            }

            foreach (XElement node in comments.Elements("comment"))
            {
                var comment = new Comment
                {
                    ID = ReadAttribute(node, "id"),
                    Author = ReadValue(node, "author"),
                    Email = ReadValue(node, "email"),
                    IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
                    Content = ReadValue(node, "content"),
                    PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01"),
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                };

                post.Comments.Add(comment);
            }
        }

        private static string ReadAttribute(XElement element, XName name, string defaultValue = "") =>
            element.Attribute(name) is null ? defaultValue : element.Attribute(name)?.Value ?? defaultValue;

        private static string ReadValue(XElement doc, XName name, string defaultValue = "") =>
            doc.Element(name) is null ? defaultValue : doc.Element(name)?.Value ?? defaultValue;

        private string GetFilePath(Post post) => Path.Combine(_folder, $"{post.Id}.xml");

        private void Initialize()
        {
            LoadPosts();
            SortCache();
        }

        private void LoadPosts()
        {
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(_folder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                XElement doc = XElement.Load(file);

                var post = new Post
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Title = ReadValue(doc, "title"),
                    Excerpt = ReadValue(doc, "excerpt"),
                    Content = ReadValue(doc, "content"),
                    Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                    PubDate = DateTime.Parse(ReadValue(doc, "pubDate"), CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal),
                    LastModified = DateTime.Parse(
                        ReadValue(
                            doc,
                            "lastModified",
                            DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
                };

                LoadCategories(post, doc);
                LoadComments(post, doc);
                _cache.Add(post);
            }
        }
    }
}
