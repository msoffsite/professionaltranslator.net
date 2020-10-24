using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using professionaltranslator.net.Models;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read;
using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.Localization.Read.Page;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.dbo.Write.Page;

namespace professionaltranslator.net.Repository
{
    public class Page
    {
        public static async Task<Models.Page> Item(string site, string name)
        {
            try
            {
                if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(name))) return null;
                Tables.dbo.Page page = await dbRead.Page.Item(site, name);
                if (page == null) return null;
                Models.Image image = page.ImageId.HasValue ? await Image.Item(page.ImageId.Value) : null;
                List<Tables.Localization.Page> localizedList = await dbLocalizedRead.List(page.Id);
                var output = new Models.Page
                {
                    Id = page.Id,
                    CanHaveImage = page.CanHaveImage,
                    IsService = page.IsService,
                    Name = page.Name,
                    Image = image,
                    Localization = localizedList.Select(n => new Models.Localized.Page
                    {
                        Lcid = n.Lcid,
                        Title = n.Title,
                        Html = n.Html
                    }).ToList()
                };
                return output;
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public static async Task<List<Models.Page>> List(string site)
        {
            List<Task<Models.Page>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<Models.Page>();
            var output = new List<Models.Page>();
            for (int i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                Models.Page item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<Models.Page>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<Models.Page>>();
            List<Tables.dbo.Page> list = await dbRead.Page.List(site);
            return Complete(list);
        }

        private static List<Task<Models.Page>> Complete(IEnumerable<Tables.dbo.Page> inputList)
        {
            return inputList.Select(async n => new Models.Page
            {
                Id = n.Id,
                Name = n.Name,
                IsService = n.IsService,
                CanHaveImage = n.CanHaveImage,
                Image = n.ImageId.HasValue ? await Image.Item(n.ImageId.Value) : null,
            }).ToList();
        }

        public static async Task<string> Save(string site, Models.Page inputItem)
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
                if (imageSaveStatus == SaveStatus.Failed.ToString()) throw new Exception("Image failed to save.");
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

            SaveStatus output = await dbWrite.Item(saveItem);
            if (output == SaveStatus.Failed) return output.ToString();

            var saveLocalizationFailed = false;
            foreach (Tables.Localization.Page saveLocalization in inputItem.Localization.Select(localizedPage => new Tables.Localization.Page()))
            {
                saveLocalization.Id = saveItem.Id;
                SaveStatus saveStatus = await DatabaseOperations.Localization.Write.Page.Item(saveLocalization);
                saveLocalizationFailed = saveStatus == SaveStatus.Failed;
                if (saveLocalizationFailed) break;
            }

            if (saveLocalizationFailed) output = SaveStatus.Failed;
            return output.ToString();
        }
    }
}
