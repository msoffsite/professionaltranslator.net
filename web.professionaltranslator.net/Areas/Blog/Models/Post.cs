using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace web.professionaltranslator.net.Areas.Blog.Models
{
    public class Post
    {
        public IList<string> Categories { get; } = new List<string>();

        public IList<Comment> Comments { get; } = new List<Comment>();

        public string Content { get; set; } = string.Empty;

        public string Excerpt { get; set; } = string.Empty;

        public string Id { get; set; } = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

        public bool IsPublished { get; set; } = true;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public string Slug { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string OgImage { get; set; } = string.Empty;

        public static string CreateSlug(string title)
        {
            title = title?.ToLowerInvariant().Replace(
                Constants.Space, Constants.Dash, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays) =>
            PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;

        public string GetEncodedLink() => $"/blog/{WebUtility.UrlEncode(Slug)}/";

        public string GetLink() => $"/blog/{Slug}/";

        public bool IsVisible() => PubDate <= DateTime.UtcNow && IsPublished;

        public string RenderContent()
        {
            string result = Content;

            // Set up lazy loading of images/iframes
            if (string.IsNullOrEmpty(result)) return result;
            
            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            const string video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            
            result = Regex.Replace(
                result,
                @"\[youtube:(.*?)\]",
                m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));

            return result;
        }

        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (char entry in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(entry);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(entry);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (string reservedCharacter in reservedCharacters)
            {
                text = text.Replace(reservedCharacter, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            return text;
        }
    }
}
