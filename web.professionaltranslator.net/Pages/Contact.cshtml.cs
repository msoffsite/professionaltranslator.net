using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Models.ProfessionalTranslator.Net;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using web.professionaltranslator.net.Models;
using Client = Repository.ProfessionalTranslator.Net.Client;
using ClientModel = Models.ProfessionalTranslator.Net.Client;
using UploadModel = Models.ProfessionalTranslator.Net.Upload.Client;
using DataModel = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Model = web.professionaltranslator.net.Models.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;
using Exception = System.Exception;
using Result = Repository.ProfessionalTranslator.Net.Result;

namespace web.professionaltranslator.net.Pages
{
    public class ContactModel : Base
    {
        private readonly IWebHostEnvironment _environment;

        public ContactModel(SiteSettings siteSettings, IWebHostEnvironment environment)
        {
            SiteSettings = siteSettings;
            _environment = environment;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Contact");
            return Item == null ? NotFound() : (IActionResult)Page();
        }

        public async Task<ActionResult> OnPostEmailAddressChange()
        {
            var stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string requestBody = await reader.ReadToEndAsync();

            if (requestBody.Length <= 0) throw new IndexOutOfRangeException("requestBody is empty.");

            var obj = JsonConvert.DeserializeObject<InitializeClient>(requestBody);
            if (obj == null) throw new NullReferenceException("Model could not be derived from JSON object.");
            ClientModel dbClientModel = await Client.Item(obj.EmailAddress) ?? new ClientModel
            {
                EmailAddress = obj.EmailAddress,
                Id = Guid.NewGuid(),
                Name = obj.Name,
                Uploads = new List<UploadModel>()
            };
            Session.Set<ClientModel>(HttpContext.Session, Session.Key.ClientDataModel, dbClientModel);
            return new JsonResult(new Result());
        }

        public async Task<ActionResult> OnPostSend()
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
                
                var obj = JsonConvert.DeserializeObject<Model>(requestBody);
                if (obj == null) throw new NullReferenceException("Model could not be derived from JSON object.");

                string messageHtml = obj.Message.Replace(Environment.NewLine, "<br/></br/>");
                obj.Message = messageHtml;

                var body = new StringBuilder();
                body.Append("<b>Name:</b> " + obj.Name);
                body.Append("<br/>");
                body.Append("<b>Email Address:</b> " + obj.EmailAddress);
                body.Append("<br/>");
                body.Append("<b>Title:</b> " + obj.TranslationType);
                body.Append("<br/>");
                body.Append("<b>Genre:</b> " + obj.SubjectMatter);
                body.Append("<br/>");
                body.Append("<b>Translation Type:</b> " + obj.TranslationDirection);
                body.Append("<br/>");
                body.Append("<b>Word Count:</b> " + $"{obj.WordCount:n0}");
                body.Append("<br/>");
                body.Append("<b>Message:</b>");
                body.Append("<br/><br />");
                body.Append(messageHtml);

                var dbClientModel = Session.Get<ClientModel>(HttpContext.Session, Session.Key.ClientDataModel);

                var dataModel = new DataModel
                {
                    ClientId = dbClientModel.Id,
                    TranslationDirection = obj.TranslationDirection,
                    TranslationType = obj.TranslationType,
                    SubjectMatter = obj.SubjectMatter,
                    WordCount = obj.WordCount,
                    Message = messageHtml,
                    ClientUploads = dbClientModel.Uploads,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid()
                };

                result = await Data.Save(SiteSettings.Site, dataModel, dbClientModel);

                Session.Set<Guid>(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId);

                var toList = new List<MailAddress>
                {
                    new MailAddress(SiteSettings.DefaultTo, SiteSettings.DefaultToDisplayName),
                    new MailAddress(obj.EmailAddress, obj.EmailAddress)
                };

                Smtp.SendMail(SiteSettings, toList, "Translation Inquiry", body.ToString(), Smtp.BodyType.Html, Smtp.SslSetting.Off);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return new JsonResult(result);
        }

        public async Task<ActionResult> OnPostUpload(IFormFile[] files)
        {
            var result = new Result();

            var fileNames = new List<string>();
            if (files != null && files.Length > 0)
            {
                foreach (IFormFile file in files)
                {
                    string path = Path.Combine(_environment.WebRootPath, "uploads", file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    fileNames.Add(file.FileName);
                }
            }
            return new JsonResult(result);
        }
    }
}
