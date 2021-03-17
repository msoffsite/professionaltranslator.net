using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Repository.ProfessionalTranslator.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ImageSharp = SixLabors.ImageSharp;
namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class EditPortfolioModel : Base
    {
        private const string PageName = "EditTestimonial";
        private readonly IHostEnvironment _environment;

        [BindProperty(SupportsGet = true)]
        public Guid QueryId { get; set; } = Guid.Empty;

        public EditPortfolioModel(SiteSettings siteSettings, AdminPortfolioSettings adminPortfolioSettings, IHostEnvironment environment)
        {
            SiteSettings = siteSettings;
            AdminPortfolioSettings = adminPortfolioSettings;
            _environment = environment;
        }

        public void OnGet()
        {
            
        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<ActionResult> OnPostAsync()
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
