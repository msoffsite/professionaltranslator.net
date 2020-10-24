using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read;
using dbReadLog = professionaltranslator.net.Repository.DatabaseOperations.Log.Read.Exception;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.Log.Write.Exception;

namespace professionaltranslator.net.Repository
{
    public class Exception
    {
        public static async Task<Models.Log.Exception> Item(Guid id)
        {
            Tables.Log.Exception exception = await dbReadLog.Item(id);
            if (exception == null) return null;
            var output = new Models.Log.Exception
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
        
        public static async Task<List<Models.Log.Exception>> List(string site)
        {
            List<Tables.Log.Exception> list = await dbReadLog.List(site);
            return list.Select(n => new Models.Log.Exception
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
            var exception = new Models.Log.Exception(inputItem, $"Repository.{className}");
            string exceptionSaveStatus = await Save(site, exception);
            if (exceptionSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Could not log exception.");
        }

        public static async Task<string> Save(string site, Models.Log.Exception inputItem)
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
