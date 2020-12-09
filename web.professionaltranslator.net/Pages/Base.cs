﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GlobalModel = Models.ProfessionalTranslator.Net.Page;
using Data = Repository.ProfessionalTranslator.Net.Page;

namespace web.professionaltranslator.net.Pages
{
    public class Base : PageModel
    {
        internal SiteSettings Configuration;

        public Models.Page Item { get; set; }

        internal async Task<Models.Page> Get(SiteSettings configuration, string page)
        {
            var output = new Models.Page();
            GlobalModel basePage = await Data.Item(configuration.Site, page);
            if ((basePage == null) || (!basePage.Bodies.Any()) || (!basePage.Headers.Any())) return output;

            basePage.Bodies = basePage.Bodies.Where(x => x.Lcid == configuration.Lcid).ToList();
            basePage.Headers = basePage.Headers.Where(x => x.Lcid == configuration.Lcid).ToList();
            
            output.Body = basePage.Bodies.Any() ? basePage.Bodies[0].Html : "Missing Body";
            output.Title = basePage.Bodies.Any() ? basePage.Bodies[0].Title : "Missing Title";
            output.Header = basePage.Headers.Any() ? basePage.Headers[0].Html : "Missing Header";
            output.Image = basePage.Image;
            return output;
        }
    }
}
