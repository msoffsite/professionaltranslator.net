using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using web.professionaltranslator.net.Extensions;

using Model = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;

namespace web.professionaltranslator.net.Pages
{
    public class InquiryResultModel : Base
    {
        public Model InquiryItem { get; set; }

        public InquiryResultModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            Guid? inquiryResultId = Session.Get(HttpContext.Session, Session.Key.InquiryResult);
            if (!inquiryResultId.HasValue) return NotFound();
            InquiryItem = await Data.Item(inquiryResultId.Value);
            Item = await new Base().Get(Configuration, "InquiryResult");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
