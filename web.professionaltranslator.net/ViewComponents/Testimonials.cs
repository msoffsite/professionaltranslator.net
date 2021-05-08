using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.ProfessionalTranslator.Net;
using Data = Repository.ProfessionalTranslator.Net.Testimonial;

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
