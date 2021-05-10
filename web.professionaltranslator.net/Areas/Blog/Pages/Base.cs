using Microsoft.AspNetCore.Authorization;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Areas.Blog.Pages
{
    public class Base : net.Pages.Base
    {
        internal Area Blog = Area.Blog;
    }
}