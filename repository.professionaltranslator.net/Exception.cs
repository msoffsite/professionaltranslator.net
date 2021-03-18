using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbReadLog = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Read.Exception;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write.Exception;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Exception
    {
        public static async Task<models.Log.Exception> Item(Guid id)
        {
            Tables.Log.Exception exception = await dbReadLog.Item(id);
            if (exception == null) return null;
            var output = new models.Log.Exception
            {
                Id = exception.Id,
                Message = exception.Message,
                Stacktrace = exception.Stacktrace,
                Type = exception.Type,
                Class = exception.Class,
                DateCreated = exception.DateCreated
            };
            return output;
        }
        
        public static async Task<List<models.Log.Exception>> List(string site)
        {
            List<Tables.Log.Exception> list = await dbReadLog.List(site);
            return list.Select(n => new models.Log.Exception
            {
                Id = n.Id,
                Message = n.Message,
                Stacktrace = n.Stacktrace,
                Type = n.Type,
                Class = n.Class,
                DateCreated = n.DateCreated
            }).ToList();
        }

        internal static async Task Save(string site, System.Exception inputItem, string className)
        {
            var exception = new models.Log.Exception(inputItem, $"Repository.{className}");
            await Save(site, exception);
        }

        public static async Task<Result> Save(string site, models.Log.Exception inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(SaveStatus.Failed, "Exception cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(SaveStatus.Failed, "No site was found with that name.");
            }

            if (string.IsNullOrEmpty(inputItem.Message)) messages.Add("Message cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Type)) messages.Add("Type cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Class))
            {
                messages.Add("Class cannot be empty.");
            }
            else if (inputItem.Class.Length > 2048)
            {
                messages.Add("Class must be 2048 characters or fewer.");
            }

            if (messages.Any())
            {
                return new Result(SaveStatus.Failed, messages);
            }

            var convertItem = new Tables.Log.Exception
            {
                Id = inputItem.Id,
                SiteId = siteItem.Id,
                Message = inputItem.Message,
                Stacktrace = inputItem.Stacktrace,
                Type = inputItem.Type,
                Class = inputItem.Class,
                DateCreated = inputItem.DateCreated ?? DateTime.Now
            };

            Result saveResult = await dbWrite.Item(site, convertItem);
            return saveResult;
        }
    }
}
