using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web.professionaltranslator.net.Models;

namespace web.professionaltranslator.net.Services
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
