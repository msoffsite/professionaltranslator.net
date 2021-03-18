using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;

using Model = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;

namespace web.professionaltranslator.net.Pages
{
    public class InquiryResultModel : Base
    {
        public Model InquiryItem { get; set; }

        public InquiryResultModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Guid? inquiryResultId = Session.Get(HttpContext.Session, Session.Key.InquiryResult);
            if (!inquiryResultId.HasValue) return BadRequest("InquiryResult couldn't be located.");
            InquiryItem = await Data.Item(inquiryResultId.Value);
            if (InquiryItem == null) return BadRequest("InquiryItem couldn't be located.");
            Item = await new Base().Get(SiteSettings, Area.Root, "InquiryResult");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
