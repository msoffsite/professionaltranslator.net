using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Repository.ProfessionalTranslator.Net.Tables.Localization;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbLocalizedRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Subscriber;
using models = Models.ProfessionalTranslator.Net;
using Nullable = Repository.ProfessionalTranslator.Net.Conversions.Nullable;

namespace Repository.ProfessionalTranslator.Net
{
    public class Subscriber
    {
        public static async Task<Result> Delete(string site, Guid? id)
        {
            var messages = new List<string>();

            Rules.StringRequired(site, "Site", ref messages);
            Rules.GuidHasValue(id, "Id", ref messages);

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            // ReSharper disable once PossibleInvalidOperationException
            return await dbWrite.Delete(site, id.Value);
        }

        public static async Task<Result> Delete(string site, string emailAddress)
        {
            var messages = new List<string>();

            Rules.StringRequired(site, "Site", ref messages);
            Rules.StringRequired(emailAddress, "EmailAddress", ref messages);

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            return await dbWrite.Delete(site, emailAddress);
        }

        public static async Task<models.Subscriber> Item(string site, Guid? id)
        {
            if ((!id.HasValue) || (string.IsNullOrWhiteSpace(site))) return null;
            Tables.dbo.Subscriber item = await dbRead.Subscriber.Item(id.Value);
            return await Item(site, item);
        }

        public static async Task<models.Subscriber> Item(string site, Area area, string emailAddress)
        {
            if ((string.IsNullOrWhiteSpace(site)) || (string.IsNullOrWhiteSpace(emailAddress))) return null;
            int areaId = Enumerators.Values.Area(area);
            Tables.dbo.Subscriber item = await dbRead.Subscriber.Item(site, areaId, emailAddress);
            return await Item(site, item);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task<models.Subscriber> Item(string site, Tables.dbo.Subscriber inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }

            try
            {
                var output = new models.Subscriber
                {
                    Id = inputItem.Id,
                    FirstName = inputItem.FirstName,
                    LastName = inputItem.LastName,
                    EmailAddress = inputItem.EmailAddress
                };
                return output;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "Repository.ProfessionalTranslator.Net.Subscriber");
                return null;
            }
        }

        public static async Task<List<models.Subscriber>> List(string site, Area area)
        {
            int areaId = Enumerators.Values.Area(area);
            List<Task<models.Subscriber>> taskList = await TaskList(site, areaId);
            if (taskList.Count == 0) return new List<models.Subscriber>();
            var output = new List<models.Subscriber>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Subscriber item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<models.Subscriber>>> TaskList(string site, int areaId)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Subscriber>>();
            List<Tables.dbo.Subscriber> list = await dbRead.Subscriber.List(site, areaId);
            return Complete(list);
        }

        private static List<Task<models.Subscriber>> Complete(IEnumerable<Tables.dbo.Subscriber> inputList)
        {   
#pragma warning disable 1998
            return inputList.Select(async n => new models.Subscriber
#pragma warning restore 1998
            {
                Id = n.Id,
                FirstName = n.FirstName,
                LastName = n.LastName,
                EmailAddress = n.EmailAddress
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
        /// <param name="inputItem">Subscriber object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, Area area, models.Subscriber inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Subscriber cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Rules.StringRequiredMaxLength(inputItem.FirstName, "Name", 50, ref messages);
            Rules.StringRequiredMaxLength(inputItem.LastName, "Name", 50, ref messages);
            if (Rules.StringRequiredMaxLength(inputItem.EmailAddress, "Email Address", 255, ref messages) ==
                Rules.Passed.Yes)
            {
                Rules.ValidateEmailAddress(inputItem.EmailAddress, "Email Address", ref messages);
            }

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            models.Subscriber existingItem = await Item(site, area, inputItem.EmailAddress);
            Guid returnId = existingItem?.Id ?? Guid.NewGuid();
            int areaId = Enumerators.Values.Area(area);
            var saveItem = new Tables.dbo.Subscriber
            {
                Id = returnId,
                SiteId = siteItem.Id,
                AreaId = areaId,
                FirstName = inputItem.FirstName,
                LastName = inputItem.LastName,
                EmailAddress = inputItem.EmailAddress
            };

            Result saveSubscriberResult = await dbWrite.Item(site, saveItem);
            messages = saveSubscriberResult.Messages;

            return new Result(saveSubscriberResult.Status, messages, returnId);
        }
    }
}
