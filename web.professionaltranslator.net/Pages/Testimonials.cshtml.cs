using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace web.professionaltranslator.net.Pages
{
    public class TestimonialsModel : Base
    {
        public TestimonialsModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, "Testimonials");
            return Item == null ? NotFound() : (IActionResult) Page();
        }
    }
}