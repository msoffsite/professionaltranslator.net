using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.ProfessionalTranslator.Net;
using Data = Repository.ProfessionalTranslator.Net.Work;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class TestimonialModel : Base
    {
        private const string PageName = "TestimonialSelectWork";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int WithTestimonials { get; set; } = 0;

        public int Count { get; set; } = -1;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, SiteSettings.PagingSizeSelectWork));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;

        public List<Work> Thumbnails { get; set; }

        public TestimonialModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Admin, PageName);
            if (WithTestimonials == 0)
            {
                Thumbnails = await Data.ListWithoutTestimonials(SiteSettings.Site, (CurrentPage - 1), SiteSettings.PagingSizeSelectWork);
                Count = await Data.PagingCountWithoutTestimonials(SiteSettings.Site);
            }
            else
            {
                Thumbnails = await Data.ListWithTestimonials(SiteSettings.Site, (CurrentPage - 1), SiteSettings.PagingSizeSelectWork);
                Count = await Data.PagingCountWithTestimonials(SiteSettings.Site);
            }
            
            
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
