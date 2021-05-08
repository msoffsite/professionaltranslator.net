using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using web.professionaltranslator.net.Models;

namespace web.professionaltranslator.net.Services
{
    public abstract class InMemoryBlogServiceBase : IBlogService
    {
        protected InMemoryBlogServiceBase(IHttpContextAccessor contextAccessor) => ContextAccessor = contextAccessor;

        protected List<Post> Cache { get; } = new List<Post>();

        protected IHttpContextAccessor ContextAccessor { get; }

        public abstract Task DeletePost(Post post);

        public virtual IAsyncEnumerable<string> GetCategories()
        {
            bool isAdmin = IsAdmin();

            IAsyncEnumerable<string> categories = Cache
                .Where(p => p.IsPublished || isAdmin)
                .SelectMany(post => post.Categories)
                .Select(cat => cat.ToLowerInvariant())
                .Distinct()
                .ToAsyncEnumerable();

            return categories;
        }

        public virtual Task<Post?> GetPostById(string id)
        {
            bool isAdmin = IsAdmin();
            Post post = Cache.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(
                post is null || !post.IsVisible() || !isAdmin
                ? null
                : post);
        }

        public virtual Task<Post?> GetPostBySlug(string slug)
        {
            bool isAdmin = IsAdmin();
            Post post = Cache.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(
                post is null || !post.IsVisible() || !isAdmin
                ? null
                : post);
        }

        /// <remarks>Overload for getPosts method to retrieve all posts.</remarks>
        public virtual IAsyncEnumerable<Post> GetPosts()
        {
            bool isAdmin = IsAdmin();
            return Cache.Where(p => p.IsVisible() || isAdmin).ToAsyncEnumerable();
        }

        public virtual IAsyncEnumerable<Post> GetPosts(int count, int skip = 0)
        {
            bool isAdmin = IsAdmin();

            IAsyncEnumerable<Post> posts = Cache
                .Where(p => p.IsVisible() || isAdmin)
                .Skip(skip)
                .Take(count)
                .ToAsyncEnumerable();

            return posts;
        }

        public virtual IAsyncEnumerable<Post> GetPostsByCategory(string category)
        {
            bool isAdmin = IsAdmin();

            IEnumerable<Post> posts = from p in Cache
                        where p.IsVisible() || isAdmin
                        where p.Categories.Contains(category, StringComparer.OrdinalIgnoreCase)
                        select p;

            return posts.ToAsyncEnumerable();
        }

        public abstract Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null);

        public abstract Task SavePost(Post post);

        protected bool IsAdmin() => ContextAccessor.HttpContext?.User?.Identity.IsAuthenticated ?? false;

        protected void SortCache() => Cache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
    }
}
