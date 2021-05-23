using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContentsTable = Repository.ProfessionalTranslator.Net.Tables.Localization.Page;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Page;
using models = Models.ProfessionalTranslator.Net;
using QuotesTable = Repository.ProfessionalTranslator.Net.Tables.Localization.Pages.Quote;
using FetchContents = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read.Page;
using FetchQuotes = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Pages.Read.Quote;

namespace Repository.ProfessionalTranslator.Net
{
    public class Page
    {
        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(ResultStatus.Failed, "Id must be a valid GUID.", null);
            }

            return await dbWrite.Delete(site, id.Value);
        }

        public static async Task<models.Page> Item(string site, Guid? id)
        {   
            if (!id.HasValue) return null;
            Tables.dbo.Page page = await dbRead.Page.Item(id.Value);
            return await Item(site, page);
        }

        public static async Task<models.Page> Item(string site, Area area, string name)
        {
            if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(name))) return null;
            int areaId = Enumerators.Values.Area(area);
            Tables.dbo.Page page = await dbRead.Page.Item(site, areaId, name);
            return await Item(site, page);
        }

        private static async Task<models.Page> Item(string site, Tables.dbo.Page page)
        {
            if (page == null)
            {
                return null;
            }

            try
            {
                List<QuotesTable> quotes = await FetchQuotes.List(page.Id);
                List<ContentsTable> contents = await FetchContents.List(page.Id);
                var output = new models.Page
                {
                    Id = page.Id,
                    Name = page.Name,
                    DateCreated = page.DateCreated,
                    LastModified = page.LastModified,
                    Contents = contents.Select(n => new models.Localized.Page
                    {
                        Lcid = n.Lcid,
                        Html = n.Html,
                        Title = n.Title
                    }).ToList(),
                    Quotes = quotes.Select(n => new models.Localized.Pages.Quote
                    {
                        Lcid = n.Lcid,
                        Text = n.Text
                    }).ToList()
                };
                return output;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "Repository.ProfessionalTranslator.Net.Page");
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
                Name = n.Name
            }).ToList();
        }

        /// <summary>
        /// Saves page and child items.
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site to which page is related.</param>
        /// <param name="area">Area where page resides.</param>
        /// <param name="inputItem">Page object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, Area area, models.Page inputItem)
        {
            var saveStatus = ResultStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Page cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Rules.StringRequiredMaxLength(inputItem.Name, "Name", 50, ref messages);

            if (messages.Any())
            {
                return new Result(saveStatus, messages);
            }

            models.Page existingItem = await Item(site, area, inputItem.Name);
            Guid returnId = existingItem?.Id ?? Guid.NewGuid();
            int areaId = Enumerators.Values.Area(area);
            var saveItem = new Tables.dbo.Page
            {
                Id = returnId,
                SiteId = siteItem.Id,
                AreaId = areaId,
                Name = inputItem.Name
            };

            Result savePageResult = await dbWrite.Item(site, saveItem);
            if (savePageResult.Status == ResultStatus.Failed) return new Result(savePageResult.Status, savePageResult.Messages);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            //foreach (Models.ProfessionalTranslator.Net.Localized.Page localizedPage in inputItem.Bodies)
            //{
            //    var saveLocalization = new Tables.Localization.Page
            //    {
            //        Id = saveItem.Id,
            //        Html = localizedPage.Html,
            //        Title = localizedPage.Title,
            //        Lcid = localizedPage.Lcid
            //    };
            //    Result localizedResult = await DatabaseOperations.Localization.Write.Page.Item(site, saveLocalization);
            //    if (localizedResult.Status != ResultStatus.Failed) continue;
            //    saveStatus = ResultStatus.PartialSuccess;
            //    messages.AddRange(localizedResult.Messages);
            //}

            if (saveStatus == ResultStatus.Undetermined)
            {
                saveStatus = ResultStatus.Succeeded;
            }

            return new Result(saveStatus, messages, returnId);
        }
    }
}
