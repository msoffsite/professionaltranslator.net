using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.ProfessionalTranslator.Net;
using Data = Repository.ProfessionalTranslator.Net.Work;
using Work = Models.Professionaltranslator.Net.Work;

namespace web.professionaltranslator.net.Pages
{
    public class PortfolioModel : Base
    {
        private const string PageName = "Portfolio";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; } = -1;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, Configuration.PagingSize));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;

        public List<Work> Thumbnails { get; set; }

        public PortfolioModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, PageName);
            Thumbnails = await Data.List(Configuration.Site, Display.Approved, (CurrentPage - 1), Configuration.PagingSize);
            Count = await Data.PagingCount(Configuration.Site, Display.Approved);
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
