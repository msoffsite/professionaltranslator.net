using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class EditPortfolioModel : Base
    {
        private const string PageName = "EditTestimonial";

        [BindProperty(SupportsGet = true)]
        public Guid QueryId { get; set; } = Guid.Empty;

        public void OnGet()
        {
        }
    }
}
