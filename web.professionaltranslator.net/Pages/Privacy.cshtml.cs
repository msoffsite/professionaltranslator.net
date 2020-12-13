using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace web.professionaltranslator.net.Pages
{
    public class PrivacyModel : Base
    {
        public PrivacyModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, "Privacy");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
