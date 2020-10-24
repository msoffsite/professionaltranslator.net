using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string exceptionSaveStatus = await Save(site, exception);
            if (exceptionSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Could not log exception.");
        }

        public static async Task<string> Save(string site, models.Log.Exception inputItem)
        {
            if (inputItem == null) throw new NullReferenceException("Exception cannot be null.");
            if (string.IsNullOrEmpty(inputItem.Message)) throw new ArgumentNullException(nameof(inputItem.Message), "Message cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Type)) throw new ArgumentNullException(nameof(inputItem.Type), "Type cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Class)) throw new ArgumentNullException(nameof(inputItem.Class), "Class cannot be empty.");
            if (inputItem.Class.Length > 2048) throw new ArgumentException("Class must be 2048 characters or fewer.", nameof(inputItem.Class));

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name. Cannot continue.");

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
            SaveStatus output = await dbWrite.Item(convertItem);
            return output.ToString();
        }
    }
}
