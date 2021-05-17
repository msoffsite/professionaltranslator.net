using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.ProfessionalTranslator.Net;
using GlobalModel = Models.ProfessionalTranslator.Net.Page;
using Data = Repository.ProfessionalTranslator.Net.Page;
using Page = web.professionaltranslator.net.Models.Page;

namespace web.professionaltranslator.net.Pages
{
    public class Base : PageModel
    {
        internal SiteSettings SiteSettings;

        public Page Item { get; set; }

        internal async Task<Page> Get(SiteSettings configuration, Area area, string page)
        {
            var output = new Page();
            GlobalModel basePage = await Data.Item(configuration.Site, area, page);
            if ((basePage == null) || (!basePage.Bodies.Any()) || (!basePage.Headers.Any())) return output;

            basePage.Bodies = basePage.Bodies.Where(x => x.Lcid == configuration.Lcid).ToList();
            basePage.Headers = basePage.Headers.Where(x => x.Lcid == configuration.Lcid).ToList();
            
            output.Body = basePage.Bodies.Any() ? basePage.Bodies[0].Html : "Missing Body";
            output.Title = basePage.Bodies.Any() ? basePage.Bodies[0].Title : "Missing Title";
            output.Header = basePage.Headers.Any() ? basePage.Headers[0].Html : "Missing Header";
            output.Image = basePage.Image;
            output.LastModified = basePage.LastModified;
            return output;
        }
    }
}
