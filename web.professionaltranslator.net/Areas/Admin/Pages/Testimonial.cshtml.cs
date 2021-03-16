using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.ProfessionalTranslator.Net;
using Repository.ProfessionalTranslator.Net;
using Base = web.professionaltranslator.net.Pages.Base;
using Data = Repository.ProfessionalTranslator.Net.Work;
using Work = Models.ProfessionalTranslator.Net.Work;

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

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, Configuration.PagingSizeSelectWork));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;

        public List<Work> Thumbnails { get; set; }

        public TestimonialModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(Configuration, Area.Admin, PageName);
            if (WithTestimonials == 0)
            {
                Thumbnails = await Data.ListWithoutTestimonials(Configuration.Site, (CurrentPage - 1), Configuration.PagingSizeSelectWork);
                Count = await Data.PagingCountWithoutTestimonials(Configuration.Site);
            }
            else
            {
                Thumbnails = await Data.ListWithTestimonials(Configuration.Site, (CurrentPage - 1), Configuration.PagingSizeSelectWork);
                Count = await Data.PagingCountWithTestimonials(Configuration.Site);
            }
            
            
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
