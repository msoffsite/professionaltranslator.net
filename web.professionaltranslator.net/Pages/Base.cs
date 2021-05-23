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
            if ((basePage == null) || (!basePage.Contents.Any())) return output;

            basePage.Quotes = basePage.Quotes.Where(x => x.Lcid == configuration.Lcid).ToList();
            
            output.Title = basePage.Contents[0].Title.Trim();
            output.Header = basePage.Quotes.Any() ? basePage.Quotes[0].Text.Trim() : output.Title;
            output.HeaderType = basePage.Quotes.Any() ? Enumeration.HeaderType.Quote : Enumeration.HeaderType.Title;
            output.Body = basePage.Contents[0].Html.Trim();
            output.LastModified = basePage.LastModified;
            return output;
        }
    }
}
