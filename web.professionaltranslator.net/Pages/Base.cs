using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model = Models.ProfessionalTranslator.Net.Page;
using Data = Repository.ProfessionalTranslator.Net.Page;

namespace web.professionaltranslator.net.Pages
{
    public class Base : PageModel
    {
        internal SiteSettings Configuration;

        public Model Item { get; set; }

        internal async Task<Model> Get(SiteSettings configuration, string page)
        {
            Model output = await Data.Item(configuration.Site, page);
            if ((output != null) && (output.Localization.Any()))
            {
                output.Localization = output.Localization.Where(x => x.Lcid == configuration.Lcid).ToList();
            }

            return output;
        }
    }
}
