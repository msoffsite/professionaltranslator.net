using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;
using Data = Repository.ProfessionalTranslator.Net.Work;
using Work = Models.ProfessionalTranslator.Net.Work;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class PortfolioModel : Base
    {
        private const string PageName = "PortfolioSelectWork";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int ShowApproved { get; set; } = 1;

        public int Count { get; set; } = -1;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, SiteSettings.PagingSizeSelectWork));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;

        public List<Work> Thumbnails { get; set; }

        public PortfolioModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Admin, PageName);
            if (ShowApproved == 0)
            {
                Thumbnails = await Data.List(SiteSettings.Site, Display.Unapproved, (CurrentPage - 1), SiteSettings.PagingSizeSelectWork);
                Count = await Data.PagingCount(SiteSettings.Site, Display.Unapproved);
            }
            else
            {
                Thumbnails = await Data.List(SiteSettings.Site, Display.Approved, (CurrentPage - 1), SiteSettings.PagingSizeSelectWork);
                Count = await Data.PagingCount(SiteSettings.Site, Display.Approved);
            }


            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
