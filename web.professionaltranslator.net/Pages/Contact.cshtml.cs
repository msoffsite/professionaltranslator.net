using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using web.professionaltranslator.net.Extensions;

using Model = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;
using Result = Repository.ProfessionalTranslator.Net.Result;

namespace web.professionaltranslator.net.Pages
{
    public class ContactModel : Base
    {
        public ContactModel(SiteSettings configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            var saveModel = new Model
            {
                Id = null,
                Name = "Test Inquiry",
                EmailAddress = "test@inquiry.com",
                Title = "Testing Inquiry",
                TranslationType = "English to Spanish",
                Genre = "Test",
                WordCount = 1000,
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In auctor tristique lorem, sed ullamcorper diam faucibus et.",
                DateCreated = DateTime.Now
            };
            Result result = await Data.Save(Configuration.Site, saveModel);
            Session.Set(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId.ToString());

            Item = await new Base().Get(Configuration, "Contact");
            return Item == null ? NotFound() : (IActionResult)Page();
        }
    }
}
