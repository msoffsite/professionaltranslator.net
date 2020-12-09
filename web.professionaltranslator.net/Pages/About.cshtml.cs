using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace web.professionaltranslator.net.Pages
{
    public class AboutModel : Base
    {
        public AboutModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, "About");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
