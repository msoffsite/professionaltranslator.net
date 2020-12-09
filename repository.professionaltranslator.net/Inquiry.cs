using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.ProfessionalTranslator.Net;
using dboDbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dboLogRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write.Inquiry;
using models = Models.ProfessionalTranslator.Net.Log;

namespace Repository.ProfessionalTranslator.Net
{
    public class Inquiry
    {
        private static Tables.Log.Inquiry Convert(models.Inquiry inputItem, Guid siteId)
        {
            if (inputItem == null) return null;
            var output = new Tables.Log.Inquiry
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteId,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress,
                Title = inputItem.Title,
                TranslationType = inputItem.TranslationType,
                Genre = inputItem.Genre,
                WordCount = inputItem.WordCount,
                Message = inputItem.Message,
                DateCreated = inputItem.DateCreated ?? DateTime.Now
            };
            return output;
        }

        public static async Task<models.Inquiry> Item(Guid id)
        {
            Tables.Log.Inquiry image = await dboLogRead.Inquiry.Item(id);
            return Item(image);
        }

        private static models.Inquiry Item(Tables.Log.Inquiry inputItem)
        {
            if (inputItem == null) return null;
            var output = new models.Inquiry
            {
                Id = inputItem.Id,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress,
                Title = inputItem.Title,
                TranslationType = inputItem.TranslationType,
                Genre = inputItem.Genre,
                WordCount = inputItem.WordCount,
                Message = inputItem.Message,
                DateCreated = inputItem.DateCreated
            };
            return output;
        }

        public static async Task<List<models.Inquiry>> List(string site)
        {
            List<Tables.Log.Inquiry> list = await dboLogRead.Inquiry.List(site);
            return list.Select(Item).ToList();
        }

        /// <summary>
        /// Save image. 
        /// </summary>
        /// /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site for image.</param>
        /// <param name="inputItem">Inquiry object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, models.Inquiry inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(SaveStatus.Failed, "Inquiry cannot be null.");
            }

            Tables.dbo.Site siteItem = await dboDbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(SaveStatus.Failed, "No site was found with that name.");
            }

            if (string.IsNullOrEmpty(inputItem.Name)) messages.Add("Name cannot be empty.");
            if (inputItem.Name.Length > 150) messages.Add("Name must be 150 characters or fewer.");

            if (string.IsNullOrEmpty(inputItem.EmailAddress)) messages.Add("EmailAddress cannot be empty.");
            if (inputItem.EmailAddress.Length > 256) messages.Add("EmailAddress must be 256 characters or fewer.");

            if (string.IsNullOrEmpty(inputItem.Title)) messages.Add("Title cannot be empty.");
            if (inputItem.Title.Length > 256) messages.Add("Title must be 256 characters or fewer.");

            if (string.IsNullOrEmpty(inputItem.TranslationType)) messages.Add("TranslationType cannot be empty.");
            if (inputItem.TranslationType.Length > 25) messages.Add("TranslationType must be 25 characters or fewer.");

            if (string.IsNullOrEmpty(inputItem.Genre)) messages.Add("Genre cannot be empty.");
            if (inputItem.Genre.Length > 25) messages.Add("Genre must be 25 characters or fewer.");

            if (inputItem.WordCount <= 0) messages.Add("WordCount must be more than zero.");

            if (string.IsNullOrEmpty(inputItem.Message)) messages.Add("Message cannot be empty.");

            if (messages.Any())
            {
                return new Result(SaveStatus.Failed, messages);
            }

            Tables.Log.Inquiry convertedInquiry = Convert(inputItem, siteItem.Id);
            if (convertedInquiry == null)
            {
                return new Result(SaveStatus.Failed, "Could not convert Inquiry model to table.");
            }

            Guid returnId = convertedInquiry.Id;

            Result saveInquiryResult = await dbWrite.Item(site, convertedInquiry);
            if (saveInquiryResult.Status == SaveStatus.PartialSuccess || saveInquiryResult.Status == SaveStatus.Succeeded)
            {
                saveInquiryResult.ReturnId = returnId;
            }

            return saveInquiryResult;
        }
    }
}
