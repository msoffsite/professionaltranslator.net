using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Data = Repository.ProfessionalTranslator.Net.Testimonial;
using Testimonial = Models.ProfessionalTranslator.Net.Testimonial;

namespace web.professionaltranslator.net.ViewComponents
{
    public class Testimonials : ViewComponent
    {
        internal SiteSettings Configuration;

        public Testimonials(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Testimonial> testimonials = await Data.List(Configuration.Site);
            return View(testimonials);
        }
    }
}
