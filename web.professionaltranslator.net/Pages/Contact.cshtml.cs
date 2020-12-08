using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;

using Model = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;
using Exception = System.Exception;
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
            Item = await new Base().Get(Configuration, "Contact");
            return Item == null ? NotFound() : (IActionResult)Page();
        }

        public async Task<ActionResult> OnPostSend()
        {
            var result = new Result();
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
                obj.Id = null;
                
                string messageHtml = obj.Message.Replace(Environment.NewLine, "<br/></br/>");
                obj.Message = messageHtml;

                var body = new StringBuilder();
                body.Append("<b>Name:</b> " + obj.Name);
                body.Append("<br/>");
                body.Append("<b>Email Address:</b> " + obj.EmailAddress);
                body.Append("<br/>");
                body.Append("<b>Title:</b> " + obj.Title);
                body.Append("<br/>");
                body.Append("<b>Genre:</b> " + obj.Genre);
                body.Append("<br/>");
                body.Append("<b>Translation Type:</b> " + obj.TranslationType);
                body.Append("<br/>");
                body.Append("<b>Word Count:</b> " + $"{obj.WordCount:n0}");
                body.Append("<br/>");
                body.Append("<b>Message:</b>");
                body.Append("<br/><br />");
                body.Append(messageHtml);

                result = await Data.Save(Configuration.Site, obj);
                Session.Set<Guid>(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId);

                var mail = new MailMessage
                {
                    From = new MailAddress(Configuration.Postmaster)
                };

                var toAddress = new MailAddress(Configuration.DefaultTo, Configuration.DisplayName);
                mail.To.Add(toAddress);
                toAddress = new MailAddress(obj.EmailAddress, obj.Name);
                mail.To.Add(toAddress);

                mail.Subject = "Translation Inquiry";
                mail.Body = body.ToString();
                var smtp = new SmtpClient(Configuration.SmtpServer);

                var credentials = new NetworkCredential(Configuration.Postmaster, Configuration.SmtpPassword);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credentials;
                smtp.Port = Configuration.SmtpPort;
                smtp.EnableSsl = false;
                mail.IsBodyHtml = true;
                smtp.SendAsync(mail, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                result.Status = SaveStatus.Failed;
                result.Messages.Add(ex.Message);
                result.ReturnId = null;
            }

            return new JsonResult(result);
        }
    }
}
