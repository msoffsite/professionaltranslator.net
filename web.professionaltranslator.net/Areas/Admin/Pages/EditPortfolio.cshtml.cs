using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ImageSharp = SixLabors.ImageSharp;

using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using DataModel = Models.ProfessionalTranslator.Net.Work;
using EditModel = web.professionaltranslator.net.Models.Admin.Work;
using Image = Repository.ProfessionalTranslator.Net.Image;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class EditPortfolioModel : Base
    {
        private const string PageName = "EditTestimonial";
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

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<ActionResult> OnPostImage()
        {
            Result result;

            try
            {
                if (string.IsNullOrEmpty(Upload?.FileName))
                {
                    throw new NullReferenceException("File control is empty.");
                }

                string extension = Path.GetExtension(Upload.FileName);

                var filename = QueryId.ToString();
                filename = filename + "." + extension;

                string contentRootPath = _environment.ContentRootPath;

                string tempFilePath = contentRootPath + AdminPortfolioSettings.TempFilePath + filename;
                string permanentFilePath = contentRootPath + AdminPortfolioSettings.FilePath + filename;

                string file = Path.Combine(tempFilePath);
                await using var fileStream = new FileStream(file, FileMode.Create);
                await Upload.CopyToAsync(fileStream);

                using (ImageSharp.Image image = await ImageSharp.Image.LoadAsync(tempFilePath))
                {
                    image.Mutate(x => x.Resize(0, AdminPortfolioSettings.MaxHeight));
                    await image.SaveAsync(permanentFilePath);
                }

                System.IO.File.Delete(tempFilePath);
                result = new Result(SaveStatus.Succeeded, "Portfolio uploaded.", QueryId);
            }
            catch (System.Exception ex)
            {
                result = new Result(SaveStatus.Failed, ex.Message, QueryId);
            }

            return new JsonResult(result);
        }
    }
}
