using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.ProfessionalTranslator.Net;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbLocalizedRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read.Page;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Page;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Page
    {
        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(SaveStatus.Failed, "Id must be a valid GUID.");
            }

            return await dbWrite.Delete(site, id.Value);
        }

        public static async Task<models.Page> Item(Guid? id)
        {   
            if (!id.HasValue) return null;
            Tables.dbo.Page page = await dbRead.Page.Item(id.Value);
            return await Item(page);
        }

        public static async Task<models.Page> Item(string site, string name)
        {
            if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(name))) return null;
            Tables.dbo.Page page = await dbRead.Page.Item(site, name);
            return await Item(page);
        }

        private static async Task<models.Page> Item(Tables.dbo.Page page)
        {
            try
            {
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
            catch (System.Exception ex)
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

        public static async Task<Result> Save(string site, models.Page inputItem)
        {
            var saveStatus = SaveStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(SaveStatus.Failed, "Page cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(SaveStatus.Failed, "No site was found with that name.");
            }

            Tables.dbo.Image saveImage = Image.Convert(inputItem.Image, siteItem.Id);
            if (saveImage != null)
            {
                inputItem.Image.Id = saveImage.Id;
                Result imageSaveResult = await Image.Save(site, inputItem.Image);
                if (imageSaveResult.Status == SaveStatus.Failed)
                {
                    messages = imageSaveResult.Messages;
                    saveStatus = SaveStatus.PartialSuccess;
                }
            }

            if (string.IsNullOrEmpty(inputItem.Name))
            {
                messages.Add("Name cannot be empty.");
            }

            if (inputItem.Name.Length > 20)
            {
                messages.Add("Name must be 20 characters or fewer.");
            }

            if (messages.Any())
            {
                return new Result(saveStatus, messages);
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

            Result savePageResult = await dbWrite.Item(site, saveItem);
            if (savePageResult.Status == SaveStatus.Failed) return new Result(savePageResult.Status, savePageResult.Messages);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (models.Localized.Page localizedPage in inputItem.Localization)
            {
                var saveLocalization = new Tables.Localization.Page
                {
                    Id = saveItem.Id,
                    Html = localizedPage.Html,
                    Title = localizedPage.Title,
                    Lcid = localizedPage.Lcid
                };
                Result localizedResult = await DatabaseOperations.Localization.Write.Page.Item(site, saveLocalization);
                if (localizedResult.Status != SaveStatus.Failed) continue;
                saveStatus = SaveStatus.PartialSuccess;
                messages.AddRange(localizedResult.Messages);
            }

            if (saveStatus == SaveStatus.Undetermined)
            {
                saveStatus = SaveStatus.Succeeded;
            }

            return new Result(saveStatus, messages);
        }
    }
}
