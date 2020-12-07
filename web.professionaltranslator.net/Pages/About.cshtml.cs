using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
