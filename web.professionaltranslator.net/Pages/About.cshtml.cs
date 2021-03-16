using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;

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
            Item = await new Base().Get(Configuration, Area.Root, "About");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
