using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web.professionaltranslator.net.Areas.Blog.Models
{
    public class Comment
    {
        public string Author { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool IsAdmin { get; set; } = false;

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public string GetGravatar()
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(Email.Trim().ToLowerInvariant());
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (byte t in hashBytes)
            {
                sb.Append(t.ToString("X2", CultureInfo.InvariantCulture));
            }

            return $"https://www.gravatar.com/avatar/{sb.ToString().ToLowerInvariant()}?s=60&d=blank";
        }

        public string RenderContent() => Text;
    }
}
