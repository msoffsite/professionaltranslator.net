using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Write;
using dboDbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dboLogRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write.Inquiry;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Inquiry
    {
        private static Tables.Log.Inquiry Convert(Models.ProfessionalTranslator.Net.Log.Inquiry inputItem, Guid siteId)
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

        public static async Task<Models.ProfessionalTranslator.Net.Log.Inquiry> Item(Guid id)
        {
            Tables.Log.Inquiry image = await dboLogRead.Inquiry.Item(id);
            return Item(image);
        }

        private static Models.ProfessionalTranslator.Net.Log.Inquiry Item(Tables.Log.Inquiry inputItem)
        {
            if (inputItem == null) return null;
            var output = new Models.ProfessionalTranslator.Net.Log.Inquiry
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

        public static async Task<List<Models.ProfessionalTranslator.Net.Log.Inquiry>> List(string site)
        {
            List<Tables.Log.Inquiry> list = await dboLogRead.Inquiry.List(site);
            return list.Select(Item).ToList();
        }

        /// <summary>
        /// Saves inquiry and client. 
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site for image.</param>
        /// <param name="inputItem">Inquiry object.</param>
        /// /// <param name="clientItem">Client to be associated with this inquiry.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, Models.ProfessionalTranslator.Net.Log.Inquiry inputItem, models.Client clientItem)
        {
            var saveStatus = ResultStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Inquiry cannot be null.");
            }

            Tables.dbo.Site siteItem = await dboDbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Rules.StringRequiredMaxLength(inputItem.TranslationType, "Translation Type", 25, ref messages);
            Rules.StringRequiredMaxLength(inputItem.TranslationDirection, "Translation Direction", 25, ref messages);
            Rules.StringRequiredMaxLength(inputItem.SubjectMatter, "SubjectMatter", 50, ref messages);
            Rules.MinIntValue(inputItem.WordCount, "Word Count", 1, ref messages);

            Rules.StringRequired(inputItem.Message, "Message", ref messages);

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            Tables.Log.Inquiry convertedInquiry = Convert(inputItem, siteItem.Id);
            if (convertedInquiry == null)
            {
                return new Result(ResultStatus.Failed, "Could not convert Inquiry model to table.");
            }

            Guid returnId = convertedInquiry.Id;

            Result saveInquiryResult = await dbWrite.Item(site, convertedInquiry);
            if (saveInquiryResult.Status == ResultStatus.PartialSuccess || saveInquiryResult.Status == ResultStatus.Succeeded)
            {
                saveInquiryResult.ReturnId = returnId;
            }
            else
            {
                saveStatus = saveInquiryResult.Status;
            }

            if (saveStatus == ResultStatus.Undetermined)
            {
                foreach (Models.ProfessionalTranslator.Net.Upload.Client uploads in clientItem.Uploads)
                {
                    Result uploadResult = await ClientInquiry.Item(uploads.Id, returnId);
                    if (uploadResult.Status != ResultStatus.Failed) continue;
                    saveStatus = ResultStatus.Failed;
                    messages.AddRange(uploadResult.Messages);
                }
            }

            if (saveStatus == ResultStatus.Undetermined)
            {
                saveStatus = ResultStatus.Succeeded;
            }

            return new Result(saveStatus, messages, returnId);
        }
    }
}
