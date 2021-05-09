#nullable enable
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WilderMinds.MetaWeblog;

namespace web.professionaltranslator.net.Services
{
    public class MetaWeblogService : IMetaWeblogProvider
    {
        private readonly IBlogService _blog;

        private readonly IConfiguration _config;

        private readonly IHttpContextAccessor _context;

        public MetaWeblogService(
            IBlogService blog,
            IConfiguration config,
            IHttpContextAccessor context)
        {
            _blog = blog;
            _config = config;
            _context = context;
        }

        public Task<int> AddCategoryAsync(string key, string username, string password, NewCategory category)
        {
            ValidateUser(username);

            throw new NotImplementedException();
        }

        public Task<string> AddPageAsync(string blogId, string username, string password, Page page, bool publish)
        {
            ValidateUser(username);

            throw new NotImplementedException();
        }

        public async Task<string> AddPostAsync(string blogId, string username, string password, Post post, bool publish)
        {
            ValidateUser(username);

            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var newPost = new Models.Post
            {
                Title = post.title,
                Slug = !string.IsNullOrWhiteSpace(post.wp_slug) ? post.wp_slug : Models.Post.CreateSlug(post.title),
                Excerpt = post.mt_excerpt,
                Content = post.description,
                IsPublished = publish
            };

            post.categories.ToList().ForEach(newPost.Categories.Add);

            if (post.dateCreated != DateTime.MinValue)
            {
                newPost.PubDate = post.dateCreated;
            }

            await _blog.SavePost(newPost).ConfigureAwait(false);

            return newPost.Id;
        }

        public Task<bool> DeletePageAsync(string blogId, string username, string password, string pageId)
        {
            ValidateUser(username);

            throw new NotImplementedException();
        }

        public async Task<bool> DeletePostAsync(string key, string postId, string username, string password, bool publish)
        {
            ValidateUser(username);

            Models.Post? post = await _blog.GetPostById(postId).ConfigureAwait(false);
            if (post is null)
            {
                return false;
            }

            await _blog.DeletePost(post).ConfigureAwait(false);
            return true;
        }

        public Task<bool> EditPageAsync(string blogId, string pageId, string username, string password, Page page, bool publish)
        {
            ValidateUser(username);

            throw new NotImplementedException();
        }

        public async Task<bool> EditPostAsync(string postId, string username, string password, Post post, bool publish)
        {
            ValidateUser(username);

            Models.Post? existing = await _blog.GetPostById(postId).ConfigureAwait(false);

            if (existing is null)
            {
                return false;
            }

            existing.Title = post.title;
            existing.Slug = post.wp_slug;
            existing.Excerpt = post.mt_excerpt;
            existing.Content = post.description;
            existing.IsPublished = publish;
            existing.Categories.Clear();
            post.categories.ToList().ForEach(existing.Categories.Add);

            if (post.dateCreated != DateTime.MinValue)
            {
                existing.PubDate = post.dateCreated;
            }

            await _blog.SavePost(existing).ConfigureAwait(false);

            return true;
        }

        public Task<Author[]> GetAuthorsAsync(string blogId, string username, string password) =>
            throw new NotImplementedException();

        public async Task<CategoryInfo[]> GetCategoriesAsync(string blogId, string username, string password)
        {
            ValidateUser(username);

            return await _blog.GetCategories()
                .Select(
                    cat =>
                        new CategoryInfo
                        {
                            categoryid = cat,
                            title = cat
                        })
                .ToArrayAsync();
        }

        public Task<Page> GetPageAsync(string blogId, string pageId, string username, string password) =>
            throw new NotImplementedException();

        public Task<Page[]> GetPagesAsync(string blogId, string username, string password, int numPages) =>
            throw new NotImplementedException();

        public async Task<Post?> GetPostAsync(string postId, string username, string password)
        {
            ValidateUser(username);

            Models.Post? post = await _blog.GetPostById(postId).ConfigureAwait(false);

            return post is null ? null : ToMetaWebLogPost(post);
        }

        public async Task<Post[]> GetRecentPostsAsync(string blogId, string username, string password, int numberOfPosts)
        {
            ValidateUser(username);

            return await _blog.GetPosts(numberOfPosts)
                .Select(ToMetaWebLogPost)
                .ToArrayAsync();
        }

        public Task<Tag[]> GetTagsAsync(string blogId, string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo> GetUserInfoAsync(string key, string username, string password)
        {
            ValidateUser(username);

            throw new NotImplementedException();
        }

        public Task<BlogInfo[]> GetUsersBlogsAsync(string key, string username, string password)
        {
            ValidateUser(username);

            HttpRequest request = _context.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}";

            return Task.FromResult(
                new[]
                {
                    new BlogInfo
                    {
                        blogid ="1",
                        blogName = _config[BlogConstants.Config.Blog.Name] ?? nameof(MetaWeblogService),
                        url = url
                    }
                });
        }

        public async Task<MediaObjectInfo> NewMediaObjectAsync(string blogId, string username, string password, MediaObject mediaObject)
        {
            ValidateUser(username);

            if (mediaObject is null)
            {
                throw new ArgumentNullException(nameof(mediaObject));
            }

            byte[] bytes = Convert.FromBase64String(mediaObject.bits);
            string path = await _blog.SaveFile(bytes, mediaObject.name).ConfigureAwait(false);

            return new MediaObjectInfo { url = path };
        }

        private Post ToMetaWebLogPost(Models.Post post)
        {
            HttpRequest request = _context.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}";

            return new Post
            {
                postid = post.Id,
                title = post.Title,
                wp_slug = post.Slug,
                permalink = url + post.GetLink(),
                dateCreated = post.PubDate,
                mt_excerpt = post.Excerpt,
                description = post.Content,
                categories = post.Categories.ToArray()
            };
        }

        private void ValidateUser(string username)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            _context.HttpContext.User = new ClaimsPrincipal(identity);
        }
    }
}
