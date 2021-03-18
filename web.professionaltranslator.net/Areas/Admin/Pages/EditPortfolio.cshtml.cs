using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ImageSharp = SixLabors.ImageSharp;

using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using DataModel = Models.ProfessionalTranslator.Net.Work;
using ImageModel = Models.ProfessionalTranslator.Net.Image;
using EditModel = web.professionaltranslator.net.Models.Admin.Work;
using Image = Repository.ProfessionalTranslator.Net.Image;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class EditPortfolioModel : Base
    {
        private const string PageName = "PortfolioEditWork";
        private readonly IHostEnvironment _environment;

        [BindProperty(SupportsGet = true)]
        public Guid QueryId { get; set; } = Guid.Empty;

        private DataModel RepositoryData { get; set; }

        public EditModel Data { get; set; }

        public EditPortfolioModel(SiteSettings siteSettings, AdminPortfolioSettings adminPortfolioSettings, IHostEnvironment environment)
        {
            SiteSettings = siteSettings;
            AdminPortfolioSettings = adminPortfolioSettings;
            _environment = environment;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Admin, PageName);
            if (Item == null) { return NotFound(); }

            RepositoryData = await Work.Item(QueryId) ?? new DataModel
            {
                Id = QueryId,
                Title = string.Empty,
                Authors = string.Empty,
                Href = string.Empty,
                DateCreated = DateTime.Now,
                Display = false,
                Cover = await Image.DefaultPortfolio(SiteSettings.Site)
            };

            Session.Json.SetObject(HttpContext.Session, Session.Key.PortfolioDataModel, RepositoryData);
            Session.Set<Guid>(HttpContext.Session, Session.Key.QueryId, QueryId);

            Data = new EditModel
            {
                Cover = RepositoryData.Cover.Path,
                Author = RepositoryData.Authors,
                Display = RepositoryData.Display,
                Title = RepositoryData.Title,
                Href = RepositoryData.Href
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

                Guid? queryId = Session.Get(HttpContext.Session, Session.Key.QueryId);
                if (!queryId.HasValue)
                {
                    throw new NullReferenceException("QueryId in session has no value.");
                }

                RepositoryData = Session.Json.GetObject<DataModel>(HttpContext.Session, Session.Key.PortfolioDataModel);
                RepositoryData.Id = queryId.Value;
                RepositoryData.Authors = obj.Author;
                RepositoryData.Display = obj.Display;
                RepositoryData.Href = obj.Href;
                RepositoryData.Title = obj.Title;

                result = await Work.Save(SiteSettings.Site, RepositoryData);
                Session.Set<Guid>(HttpContext.Session, Session.Key.InquiryResult, result.ReturnId);
            }
            catch (System.Exception ex)
            {
                result = new Result(SaveStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        public async Task<ActionResult> OnPostUploadImage()
        {
            Result result;

            try
            {
                if (string.IsNullOrEmpty(UploadedFile?.FileName))
                {
                    throw new NullReferenceException("File control is empty.");
                }

                string extension = Path.GetExtension(UploadedFile.FileName);

                Guid? queryId = Session.Get(HttpContext.Session, Session.Key.QueryId);
                if (!queryId.HasValue)
                {
                    throw new NullReferenceException("QueryId in session has no value.");
                }

                var filename = queryId.Value.ToString();
                filename += extension;

                string contentRootPath = _environment.ContentRootPath;

                string tempFilePath = contentRootPath + AdminPortfolioSettings.TempFilePath + filename;
                string permanentFilePath = contentRootPath + AdminPortfolioSettings.FilePath + filename;

                string file = Path.Combine(tempFilePath);
                await using var fileStream = new FileStream(file, FileMode.Create);
                await UploadedFile.CopyToAsync(fileStream);
                fileStream.Close();
                await fileStream.DisposeAsync();

                using (ImageSharp.Image image = await ImageSharp.Image.LoadAsync(tempFilePath))
                {
                    image.Mutate(x => x.Resize(0, AdminPortfolioSettings.MaxHeight));
                    await image.SaveAsync(permanentFilePath);
                }

                System.IO.File.Delete(tempFilePath);

                string imageWebPath = AdminPortfolioSettings.ImageWebPath + filename;

                var dataModel = Session.Json.GetObject<DataModel>(HttpContext.Session, Session.Key.PortfolioDataModel);
                ImageModel cover = dataModel.Cover;
                cover.Path = imageWebPath;
                dataModel.Cover = cover;
                Session.Json.SetObject(HttpContext.Session, Session.Key.PortfolioDataModel, dataModel);

                result = new Result(SaveStatus.Succeeded, imageWebPath, queryId.Value);
            }
            catch (System.Exception ex)
            {
                result = new Result(SaveStatus.Failed, ex.Message, QueryId);
            }

            return new JsonResult(result);
        }
    }
}
