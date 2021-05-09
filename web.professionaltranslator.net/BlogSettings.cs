namespace web.professionaltranslator.net
{
    public class BlogSettings
    {
        public int CommentsCloseAfterDays { get; set; } = 10;

        public bool DisplayComments { get; set; } = true;

        public PostListView ListView { get; set; } = PostListView.TitlesAndExcerpts;

        public string Owner { get; set; } = "Cinta Garcia de la Rosa";

        public int PostsPerPage { get; set; } = 4;
    }
}
