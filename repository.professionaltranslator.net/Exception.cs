using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Professionaltranslator.Net;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbReadLog = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Read.Exception;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write.Exception;
using models = Models.Professionaltranslator.Net;

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
            SaveStatus saveStatus;
            var errorMessages = new List<string>();

            if (inputItem != null && string.IsNullOrEmpty(inputItem.Message)) errorMessages.Add("Message cannot be empty.");
            if (inputItem != null && string.IsNullOrEmpty(inputItem.Type)) errorMessages.Add("Type cannot be empty.");
            if (inputItem != null && string.IsNullOrEmpty(inputItem.Class)) errorMessages.Add("Class cannot be empty.");
            if (inputItem != null && inputItem.Class.Length > 2048) errorMessages.Add("Class must be 2048 characters or fewer.");

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name.");

            if (inputItem == null)
            {
                errorMessages.Add("Exception cannot be null.");
                saveStatus = SaveStatus.Failed;
                return new Result(saveStatus, errorMessages);
            }

            var convertItem = new Tables.Log.Exception
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteItem.Id,
                Message = inputItem.Message,
                Stacktrace = inputItem.Stacktrace,
                Type = inputItem.Type,
                Class = inputItem.Class,
                DateCreated = inputItem.DateCreated ?? DateTime.Now
            };

            SaveStatus dbSaveStatus = await dbWrite.Item(site, convertItem);
            if (dbSaveStatus == SaveStatus.Failed) throw new System.Exception("Could not log exception.");
            saveStatus = errorMessages.Any() ? SaveStatus.Failed : dbSaveStatus;
            return new Result(saveStatus, errorMessages);
        }
    }
}
