using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbUploadRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Read.Client;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Client;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Client
    {
        public static async Task<models.Client> Item(Guid? id)
        {
            if (!id.HasValue) return null;
            Tables.dbo.Client client = await dbRead.Client.Item(id.Value);
            return await Item(client, null);
        }

        public static async Task<models.Client> Item(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) return null;
            Tables.dbo.Client client = await dbRead.Client.Item(emailAddress);
            return client == null ? null : await Item(client, null);
        }

        public static async Task<models.Client> Item(Guid? id, Guid? inquiryId)
        {
            if (!id.HasValue || !inquiryId.HasValue) return null;
            Tables.dbo.Client client = await dbRead.Client.Item(id.Value);
            return await Item(client, inquiryId);
        }

        private static async Task<models.Client> Item(Tables.dbo.Client client, Guid? inquiryId)
        {
            try
            {
                List<Tables.Upload.Client> uploads = inquiryId.HasValue ? await dbUploadRead.List(client.Id, inquiryId) : await dbUploadRead.List(client.Id);
                var output = new models.Client
                {
                    Id = client.Id,
                    Name = client.Name,
                    EmailAddress = client.EmailAddress,
                    Uploads = uploads.Select(n => new models.Upload.Client
                    {
                        Id = n.Id,
                        GeneratedFilename = n.GeneratedFilename,
                        OriginalFilename = n.OriginalFilename
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

        public static async Task<List<models.Client>> List(string site)
        {
            List<Task<models.Client>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<models.Client>();
            var output = new List<models.Client>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Client item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<models.Client>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Client>>();
            List<Tables.dbo.Client> list = await dbRead.Client.List(site);
            return Complete(list);
        }

        private static List<Task<models.Client>> Complete(IEnumerable<Tables.dbo.Client> clientList)
        {
            return clientList.Select(async n => await Item(n, null)).ToList();
        }

        /// <summary>
        /// Saves client and client uploads.
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site related to testimonial.</param>
        /// <param name="inputItem">Client object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, models.Client inputItem)
        {
            var saveStatus = ResultStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Client cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Rules.StringRequiredMaxLength(inputItem.Name, "Name", 150, ref messages);

            if (Rules.StringRequiredMaxLength(inputItem.EmailAddress, "Email Address", 256, ref messages) ==
                Rules.Passed.Yes)
            {
                Rules.ValidateEmailAddress(inputItem.EmailAddress, "Email Address", ref messages);
            }

            foreach (models.Upload.Client uploads in inputItem.Uploads)
            {
                Rules.StringRequiredMaxLength(uploads.GeneratedFilename, "Generated filename", 45, ref messages);
                Rules.StringRequiredMaxLength(uploads.OriginalFilename, "Original filename", 256, ref messages);
            }

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            var saveItem = new Tables.dbo.Client
            {
                Id = inputItem.Id,
                SiteId = siteItem.Id,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress
            };

            Result saveClientResult = await dbWrite.Item(site, saveItem);
            if (saveClientResult.Status == ResultStatus.Failed)
            {
                return saveClientResult;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (models.Upload.Client uploads in inputItem.Uploads)
            {
                var saveUpload = new Tables.Upload.Client
                {
                    Id = uploads.Id,
                    ClientId = saveItem.Id,
                    GeneratedFilename = uploads.GeneratedFilename,
                    OriginalFilename = uploads.OriginalFilename
                };
                Result uploadResult = await DatabaseOperations.Upload.Write.Client.Item(saveUpload);
                if (uploadResult.Status != ResultStatus.Failed) continue;
                saveStatus = ResultStatus.PartialSuccess;
                messages.AddRange(uploadResult.Messages);
            }

            if (saveStatus == ResultStatus.Undetermined)
            {
                saveStatus = ResultStatus.Succeeded;
            }

            return new Result(saveStatus, messages, inputItem.Id);

        }
    }
}
