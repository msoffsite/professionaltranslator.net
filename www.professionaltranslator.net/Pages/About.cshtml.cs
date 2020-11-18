using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace www.professionaltranslator.net.Pages
{
    public class AboutModel : Base
    {
        //private readonly ILogger<IndexModel> _logger;

        public AboutModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        //public HomeModel(SiteSettings configuration, ILogger<IndexModel> logger)
        //{
        //    _configuration = configuration;
        //    _logger = logger;
        //}

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, "About");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
