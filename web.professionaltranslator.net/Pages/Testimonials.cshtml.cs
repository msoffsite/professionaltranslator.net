using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Pages
{
    public class TestimonialsModel : Base
    {
        public TestimonialsModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Testimonials");
            return Item == null ? NotFound() : (IActionResult) Page();
        }
    }
}