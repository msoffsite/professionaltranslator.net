using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace web.professionaltranslator.net.Pages
{
    public class HomeModel : Base
    {
        //private readonly ILogger<IndexModel> _logger;

        public HomeModel(SiteSettings configuration)
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
            Item = await new Base().Get(Configuration, "Home");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
