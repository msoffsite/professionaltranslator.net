using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Pages
{
    public class CookiesModel : Base
    {
        public CookiesModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Cookies");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
