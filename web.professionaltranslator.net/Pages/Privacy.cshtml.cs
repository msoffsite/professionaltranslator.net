using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Pages
{
    public class PrivacyModel : Base
    {
        public PrivacyModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Privacy");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
