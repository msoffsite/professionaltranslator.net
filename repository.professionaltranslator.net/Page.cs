using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbLocalizedRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read.Page;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Page;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Page
    {
        public static async Task<models.Page> Item(string site, string name)
        {
            try
            {
                if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(name))) return null;
                Tables.dbo.Page page = await dbRead.Page.Item(site, name);
                if (page == null) return null;
                models.Image image = page.ImageId.HasValue ? await Image.Item(page.ImageId.Value) : null;
                List<Tables.Localization.Page> localizedList = await dbLocalizedRead.List(page.Id);
                var output = new models.Page
                {
                    Id = page.Id,
                    CanHaveImage = page.CanHaveImage,
                    IsService = page.IsService,
                    Name = page.Name,
                    Image = image,
                    Localization = localizedList.Select(n => new models.Localized.Page
                    {
                        Lcid = n.Lcid,
                        Title = n.Title,
                        Html = n.Html
                    }).ToList()
                };
                return output;
            }
            catch(System.Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public static async Task<List<models.Page>> List(string site)
        {
            List<Task<models.Page>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<models.Page>();
            var output = new List<models.Page>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Page item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<models.Page>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Page>>();
            List<Tables.dbo.Page> list = await dbRead.Page.List(site);
            return Complete(list);
        }

        private static List<Task<models.Page>> Complete(IEnumerable<Tables.dbo.Page> inputList)
        {
            return inputList.Select(async n => new models.Page
            {
                Id = n.Id,
                Name = n.Name,
                IsService = n.IsService,
                CanHaveImage = n.CanHaveImage,
                Image = n.ImageId.HasValue ? await Image.Item(n.ImageId.Value) : null,
            }).ToList();
        }

        public static async Task<string> Save(string site, models.Page inputItem)
        {
            if (inputItem == null) throw new NullReferenceException("Page cannot be null.");
            if (string.IsNullOrEmpty(inputItem.Name)) throw new ArgumentNullException(nameof(inputItem.Name), "Name cannot be empty.");
            if (inputItem.Name.Length > 20) throw new ArgumentException("Name must be 20 characters or fewer.", nameof(inputItem.Name));

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name. Cannot continue.");
            
            Tables.dbo.Image saveImage = Image.Convert(inputItem.Image, siteItem.Id);
            if (saveImage != null)
            {
                inputItem.Image.Id = saveImage.Id;
                string imageSaveStatus = await Image.Save(site, inputItem.Image);
                if (imageSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Image failed to save.");
            }

            var saveItem = new Tables.dbo.Page
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteItem.Id,
                CanHaveImage = inputItem.CanHaveImage,
                ImageId = saveImage?.Id ?? Conversions.Nullable.Guid(null),
                IsService = inputItem.IsService,
                Name = inputItem.Name
            };

            SaveStatus output = await dbWrite.Item(site, saveItem);
            if (output == SaveStatus.Failed) return output.ToString();

            var saveLocalizationFailed = false;
            foreach (Tables.Localization.Page saveLocalization in inputItem.Localization.Select(localizedPage => new Tables.Localization.Page()))
            {
                saveLocalization.Id = saveItem.Id;
                SaveStatus saveStatus = await DatabaseOperations.Localization.Write.Page.Item(site, saveLocalization);
                saveLocalizationFailed = saveStatus == SaveStatus.Failed;
                if (saveLocalizationFailed) break;
            }

            if (saveLocalizationFailed) output = SaveStatus.Failed;
            return output.ToString();
        }
    }
}
