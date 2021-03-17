using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using DataModel = Models.ProfessionalTranslator.Net.Testimonial;
using LocalizedDataModel = Models.ProfessionalTranslator.Net.Localized.Testimonial;
using EditModel = web.professionaltranslator.net.Models.Admin.Testimonial;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class EditTestimonialModel : Base
    {
        private const string PageName = "EditTestimonial";

        [BindProperty(SupportsGet = true)]
        public Guid QueryId { get; set; } = Guid.Empty;

        private DataModel RepositoryData { get; set; }

        public EditModel Data { get; set; }

        public EditTestimonialModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Admin, PageName);
            if (Item == null) { return NotFound(); }

            RepositoryData = await Testimonial.Item(SiteSettings.Site, QueryId) ?? new DataModel
            {
                Name = string.Empty,
                EmailAddress = string.Empty,
                Work = await Work.Item(QueryId),
                Entries = new List<LocalizedDataModel>()
            };

            if (RepositoryData.Work == null) { return NotFound(); }

            // ReSharper disable once InvertIf
            if (!RepositoryData.Entries.Any())
            {
                var testimonial = new LocalizedDataModel
                {
                    Lcid = SiteSettings.Lcid,
                    Html = string.Empty
                };
                RepositoryData.Entries.Add(testimonial);
            }
            else
            {
                LocalizedDataModel entry = RepositoryData.Entries.FirstOrDefault(x => x.Lcid == SiteSettings.Lcid);

                if (entry == null) { return NotFound(); }
                
                RepositoryData.Entries.Remove(entry);
                entry.Html = entry.Html.Replace("<p>", string.Empty).Replace("</p>", string.Empty);
                RepositoryData.Entries.Add(entry);
            }

            Session.Json.SetObject(HttpContext.Session, Session.Key.TestimonialDataModel, RepositoryData);

            Data = new EditModel
            {
                Cover = RepositoryData.Work.Cover.Path,
                Author = RepositoryData.Work.Authors,
                EmailAddress = RepositoryData.EmailAddress,
                Text = RepositoryData.Entries.FirstOrDefault(x => x.Lcid == SiteSettings.Lcid)?.Html,
                Title = RepositoryData.Work.Title
            };

            return Page();
        }

        public async Task<ActionResult> OnPostSave()
        {
            Result result;
            try
            {
                var stream = new MemoryStream();
                await Request.Body.CopyToAsync(stream);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                string requestBody = await reader.ReadToEndAsync();

                if (requestBody.Length <= 0) throw new IndexOutOfRangeException("requestBody is empty.");

                var obj = JsonConvert.DeserializeObject<EditModel>(requestBody);
                if (obj == null) throw new NullReferenceException("Model could not be derived from JSON object.");

                RepositoryData = Session.Json.GetObject<DataModel>(HttpContext.Session, Session.Key.TestimonialDataModel);
                RepositoryData.Name = obj.Author;
                RepositoryData.EmailAddress = obj.EmailAddress;

                LocalizedDataModel entry = RepositoryData.Entries.FirstOrDefault(x => x.Lcid == SiteSettings.Lcid);
                
                if (entry == null) { return NotFound(); }
                
                RepositoryData.Entries.Remove(entry);
                entry.Html = "<p>" + obj.Text.Replace(Environment.NewLine, string.Empty) + "</p>";
                RepositoryData.Entries.Add(entry);

                result = await Testimonial.Save(SiteSettings.Site, RepositoryData);
                Session.Set<Guid>(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return new JsonResult(result);
        }
    }
}
