using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Professionaltranslator.Net;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbLocalizedRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read.Testimonial;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Testimonial;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Testimonial
    {
        public static async Task<List<models.Testimonial>> List(string site)
        {
            List<Task<models.Testimonial>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<models.Testimonial>();
            var output = new List<models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<models.Testimonial>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site);
            return Complete(list, localization);
        }

        public static async Task<List<models.Testimonial>> List(string site, bool approved)
        {
            List<Task<models.Testimonial>> taskList = await TaskList(site, approved);
            if (taskList.Count == 0) return new List<models.Testimonial>();
            var output = new List<models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<models.Testimonial>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site, approved);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site, approved);
            return Complete(list, localization);
        }

        private static List<Task<models.Testimonial>> Complete(IEnumerable<Tables.dbo.Testimonial> testimonialList,
            IReadOnlyCollection<Tables.Localization.Testimonial> localizedList)
        {
            return testimonialList.Select(async n => new models.Testimonial
            {
                Id = n.Id,
                Work = await Work.Item(n.WorkId),
                Portrait = await Image.Item(n.PortraitImageId),
                Name = n.Name,
                EmailAddress = n.EmailAddress,
                DateCreated = n.DateCreated,
                Approved = n.Approved,
                Localization = localizedList.Where(x => x.Id == n.Id).Select(t => new models.Localized.Testimonial
                {
                    Lcid = t.Lcid,
                    Html = t.Html
                }).ToList()
            }).ToList();
        }

        public static async Task<Result> Save(string site, models.Testimonial inputItem)
        {
            SaveStatus saveStatus = SaveStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(SaveStatus.Failed, "Testimonial cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(SaveStatus.Failed, "No site was found with that name.");
            }

            Tables.dbo.Work convertedWork = Work.Convert(inputItem.Work, siteItem.Id);
            if (convertedWork == null)
            {
                return new Result(SaveStatus.Failed, "Could not convert work model to table.");
            }

            Result saveWorkResult = await Work.Save(site, inputItem.Work);
            if (saveWorkResult.Status == SaveStatus.Failed)
            {
                return saveWorkResult;
            }

            Tables.dbo.Image convertedPortrait = Image.Convert(inputItem.Portrait, siteItem.Id);
            if (convertedPortrait == null)
            {
                return new Result(SaveStatus.Failed, "Could not convert portrait model to table.");
            }

            Result savePortraitResult = await Image.Save(site, inputItem.Portrait);
            if (savePortraitResult.Status == SaveStatus.Failed)
            {
                return savePortraitResult;
            }

            inputItem.Portrait.Id = convertedPortrait.Id;

            if (string.IsNullOrEmpty(inputItem.Name))
            {
                messages.Add("Name cannot be empty.");
            }
            else if (inputItem.Name.Length > 100)
            {
                messages.Add("Name must be 100 characters or fewer.");
            }

            if (string.IsNullOrEmpty(inputItem.EmailAddress))
            {
                messages.Add("Email address cannot be empty.");
            }
            else if (inputItem.EmailAddress.Length > 256)
            {
                messages.Add("Email address must be 256 characters or fewer.");
            }

            if (messages.Any())
            {
                return new Result(SaveStatus.Failed, messages);
            }

            var saveItem = new Tables.dbo.Testimonial
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteItem.Id,
                WorkId = convertedWork.Id,
                PortraitImageId = convertedPortrait.Id,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress,
                Approved = inputItem.Approved
            };

            Result saveTestimonialResult = await dbWrite.Item(site, saveItem);
            if (saveTestimonialResult.Status == SaveStatus.Failed)
            {
                return saveTestimonialResult;
            }

            foreach (Tables.Localization.Testimonial saveLocalization in inputItem.Localization.Select(localizedPage =>
                new Tables.Localization.Testimonial()))
            {
                saveLocalization.Id = saveItem.Id;
                Result localizedResult = await DatabaseOperations.Localization.Write.Testimonial.Item(site, saveLocalization);
                if (localizedResult.Status != SaveStatus.Failed) continue;
                saveStatus = SaveStatus.PartialSuccess;
                messages.AddRange(localizedResult.Messages);
            }

            if (saveStatus != SaveStatus.Undetermined)
            {
                saveStatus = SaveStatus.Succeeded;
            }
            
            return new Result(saveStatus, messages);

        }
    }
}
