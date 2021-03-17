using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Pages
{
    public class HomeModel : Base
    {
        //private readonly ILogger<IndexModel> _logger;

        public HomeModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        //public HomeModel(SiteSettings configuration, ILogger<IndexModel> logger)
        //{
        //    _configuration = configuration;
        //    _logger = logger;
        //}

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Home");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
