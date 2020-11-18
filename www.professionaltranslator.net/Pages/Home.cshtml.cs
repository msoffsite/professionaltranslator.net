using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace www.professionaltranslator.net.Pages
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
            Item = await new Base().Get(Configuration, "About");
            if (Item != null)
            {
                Item.Name = "This is the about page. Content for home needed.";
            }
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
