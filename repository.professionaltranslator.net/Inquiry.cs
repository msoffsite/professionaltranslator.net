using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Id = inputItem.Id,
                SiteId = siteId,
                ClientId = inputItem.ClientId,
                TranslationType = inputItem.TranslationType,
                TranslationDirection = inputItem.TranslationDirection,
                SubjectMatter = inputItem.SubjectMatter,
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
                ClientId = inputItem.ClientId,
                TranslationType = inputItem.TranslationType,
                TranslationDirection = inputItem.TranslationDirection,
                SubjectMatter = inputItem.SubjectMatter,
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

            Rules.StringRequiredMaxLength(inputItem.TranslationType, "Translation Type", 25, ref messages);
            Rules.StringRequiredMaxLength(inputItem.TranslationDirection, "Translation Direction", 25, ref messages);
            Rules.StringRequiredMaxLength(inputItem.SubjectMatter, "SubjectMatter", 50, ref messages);
            Rules.MinIntValue(inputItem.WordCount, "Word Count", 1, ref messages);

            Rules.StringRequired(inputItem.Message, "Message", ref messages);

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
