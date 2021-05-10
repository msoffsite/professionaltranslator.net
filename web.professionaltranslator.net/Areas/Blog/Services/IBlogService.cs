#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using web.professionaltranslator.net.Areas.Blog.Models;
using web.professionaltranslator.net.Models;

namespace web.professionaltranslator.net.Areas.Blog.Services
{
    public interface IBlogService
    {
        Task DeletePost(Post post);

        IAsyncEnumerable<string> GetCategories();

        Task<Post?> GetPostById(string id);

        Task<Post?> GetPostBySlug(string slug);

        IAsyncEnumerable<Post> GetPosts();

        IAsyncEnumerable<Post> GetPosts(int count, int skip = 0);

        IAsyncEnumerable<Post> GetPostsByCategory(string category);

        Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null);

        Task SavePost(Post post);
    }
}
