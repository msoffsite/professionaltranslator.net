using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using web.professionaltranslator.net.Models;
using ClientModel = Models.ProfessionalTranslator.Net.Client;
using UploadModel = Models.ProfessionalTranslator.Net.Upload.Client;
using DataModel = Models.ProfessionalTranslator.Net.Log.Inquiry;
using Model = web.professionaltranslator.net.Models.Inquiry;
using Data = Repository.ProfessionalTranslator.Net.Inquiry;
using Exception = System.Exception;

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
            Result result;
            try
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

                Session.Json.SetObject(HttpContext.Session, Session.Key.ClientDataModel, dbClientModel);

                result = new Result(ResultStatus.Succeeded, "Client initialized.", dbClientModel.Id);
            }
            catch (Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }
            
            return new JsonResult(result);
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

                var dbClientModel = Session.Json.GetObject<ClientModel>(HttpContext.Session, Session.Key.ClientDataModel);
                dbClientModel.Name = obj.Name;

                result = await Client.Save(SiteSettings.Site, dbClientModel);
                if (result.Status == ResultStatus.Failed)
                {
                    return new JsonResult(result);
                }

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

                if (result.Status == ResultStatus.Succeeded)
                {
                    result.Messages = new List<string> { "Message saved." };
                }

                var body = new StringBuilder();
                body.Append("<b>Name:</b> " + obj.Name);
                body.Append("<br/>");
                body.Append("<b>Email Address:</b> " + obj.EmailAddress);
                body.Append("<br/>");
                body.Append("<b>Subject Matter:</b> " + obj.SubjectMatter);
                body.Append("<br/>");
                body.Append("<b>Translation Type:</b> " + obj.TranslationType);
                body.Append("<br/>");
                body.Append("<b>Translation Direction:</b> " + obj.TranslationDirection);
                body.Append("<br/>");
                body.Append("<b>Word Count:</b> " + $"{obj.WordCount:n0}");
                body.Append("<br/>");
                body.Append("<b>Message:</b>");
                body.Append("<br/><br />");
                body.Append(messageHtml);
                body.Append("<br/><br />");
                if (dbClientModel.Uploads.Count > 0)
                {
                    body.Append("The following files were uploaded:");
                    body.Append("<br />");

                    foreach (UploadModel uploadedFile in dbClientModel.Uploads)
                    {
                        body.Append(uploadedFile.OriginalFilename);
                        body.Append("<br />");
                    }

                    body.Append("<br/><br />");
                }

                Session.Set<Guid>(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId);

                var toList = new List<MailAddress>
                {
                    new MailAddress(SiteSettings.DefaultTo, SiteSettings.DefaultToDisplayName),
                    new MailAddress(obj.EmailAddress, obj.Name)
                };

                var replyToList = new List<MailAddress>
                {
                    new MailAddress(obj.EmailAddress, obj.Name)
                };

                Smtp.SendMail(SiteSettings, replyToList, toList, new List<MailAddress>(), new List<MailAddress>(), 
                    "Translation Inquiry", body.ToString(), Smtp.BodyType.Html, Smtp.SslSetting.Off);
            }
            catch (Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }

        public async Task<ActionResult> OnPostUpload(IFormFile[] files)
        {
            var result = new Result(ResultStatus.Undetermined, string.Empty, null);

            if (files == null || files.Length <= 0)
            {
                result.Status = ResultStatus.Failed;
                result.Messages = new List<string> {"No files found."};
                return new JsonResult(result);
            }

            try
            {
                var dbClientModel = Session.Json.GetObject<ClientModel>(HttpContext.Session, Session.Key.ClientDataModel);
                if (dbClientModel == null)
                {
                    result.Status = ResultStatus.Failed;
                    result.Messages = new List<string> { "Session client information not found." };
                    return new JsonResult(result);
                }

                var uploadedFiles = new List<UploadModel>();

                foreach (IFormFile file in files)
                {
                    string extension = Path.GetExtension(file.FileName);

                    string originalFilename = file.FileName;

                    UploadModel uploadedFile = await Repository.ProfessionalTranslator.Net.Upload.Client.Item(Guid.Empty, originalFilename);
                    if (uploadedFile == null)
                    {
                        var generatedId = Guid.NewGuid();
                        uploadedFile = new UploadModel
                        {
                            Id = generatedId,
                            GeneratedFilename = generatedId + extension,
                            OriginalFilename = originalFilename
                        };
                    }

                    var subfolder = $"uploads/{dbClientModel.Id}";
                    string ensureFolderPath = Path.Combine(_environment.WebRootPath, subfolder);
                    if (!Directory.Exists(ensureFolderPath))
                    {
                        Directory.CreateDirectory(ensureFolderPath);
                    }

                    string savePath = Path.Combine(_environment.WebRootPath, subfolder, uploadedFile.GeneratedFilename);
                    await using var fileStream = new FileStream(savePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                    await fileStream.DisposeAsync();
                    uploadedFiles.Add(uploadedFile);
                }

                dbClientModel.Uploads = uploadedFiles;
                Session.Json.SetObject(HttpContext.Session, Session.Key.ClientDataModel, dbClientModel);

                result.Status = ResultStatus.Succeeded;
            }
            catch (Exception ex)
            {
                result.Status = ResultStatus.Failed;
                result.Messages = new List<string> {ex.Message};
            }

            return new JsonResult(result);
        }
    }
}
