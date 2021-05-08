using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web.professionaltranslator.net.Extensions;
using Model = web.professionaltranslator.net.Models.Inquiry;
using Data = Repository.ProfessionalTranslator.Net;
using DataModel = Models.ProfessionalTranslator.Net.Log.Inquiry;
using ClientModel = Models.ProfessionalTranslator.Net.Client;

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
            DataModel dataInquiry = await Data.Inquiry.Item(inquiryResultId.Value);
            ClientModel clientModel = await Data.Client.Item(dataInquiry.ClientId, inquiryResultId);
            if (clientModel == null)
            {
                return NotFound();
            }

            InquiryItem = new Model
            {
                Name = clientModel.Name,
                EmailAddress = clientModel.EmailAddress,
                Message = dataInquiry.Message,
                SubjectMatter = dataInquiry.SubjectMatter,
                TranslationDirection = dataInquiry.TranslationDirection,
                TranslationType = dataInquiry.TranslationType,
                WordCount = dataInquiry.WordCount,
                Uploads = clientModel.Uploads.Select(x => x.OriginalFilename).ToList()
            };
            if (InquiryItem == null) return BadRequest("InquiryItem couldn't be located.");
            Item = await new Base().Get(SiteSettings, Data.Area.Root, "InquiryResult");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
