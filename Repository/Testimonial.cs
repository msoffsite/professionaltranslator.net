using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read;
using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.Localization.Read.Testimonial;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.dbo.Write.Testimonial;

namespace professionaltranslator.net.Repository
{
    public class Testimonial
    {
        public static async Task<List<Models.Testimonial>> List(string site)
        {
            List<Task<Models.Testimonial>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<Models.Testimonial>();
            var output = new List<Models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                Models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<Models.Testimonial>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<Models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site);
            return Complete(list, localization);
        }

        public static async Task<List<Models.Testimonial>> List(string site, bool approved)
        {
            List<Task<Models.Testimonial>> taskList = await TaskList(site, approved);
            if (taskList.Count == 0) return new List<Models.Testimonial>();
            var output = new List<Models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                Models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<Models.Testimonial>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<Models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site, approved);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site, approved);
            return Complete(list, localization);
        }

        private static List<Task<Models.Testimonial>> Complete(IEnumerable<Tables.dbo.Testimonial> testimonialList,
            IReadOnlyCollection<Tables.Localization.Testimonial> localizedList)
        {
            return testimonialList.Select(async n => new Models.Testimonial
            {
                Id = n.Id,
                Work = await Work.Item(n.WorkId),
                Portrait = await Image.Item(n.PortraitImageId),
                Name = n.Name,
                EmailAddress = n.EmailAddress,
                DateCreated = n.DateCreated,
                Approved = n.Approved,
                Localization = localizedList.Where(x => x.Id == n.Id).Select(t => new Models.Localized.Testimonial
                {
                    Lcid = t.Lcid,
                    Html = t.Html
                }).ToList()
            }).ToList();
        }

        public static async Task<string> Save(string site, Models.Testimonial inputItem)
        {
            if (inputItem == null) throw new NullReferenceException("Testimonial cannot be null.");
            if (string.IsNullOrEmpty(inputItem.Name))
                throw new ArgumentNullException(nameof(inputItem.Name), "Name cannot be empty.");
            if (inputItem.Name.Length > 100)
                throw new ArgumentException("Name must be 100 characters or fewer.", nameof(inputItem.Name));
            if (string.IsNullOrEmpty(inputItem.EmailAddress))
                throw new ArgumentNullException(nameof(inputItem.EmailAddress), "Email address cannot be empty.");
            if (inputItem.EmailAddress.Length > 256) throw new ArgumentException("Email address must be 256 characters or fewer.", nameof(inputItem.EmailAddress));

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
                throw new NullReferenceException("No site was found with that name. Cannot continue.");

            Tables.dbo.Work convertWork = Work.Convert(inputItem.Work, siteItem.Id);
            if (convertWork == null) throw new NullReferenceException("Work failed to convert");
            string workSaveStatus = await Work.Save(site, inputItem.Work);
            if (workSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Work failed to save.");

            Tables.dbo.Image convertPortrait = Image.Convert(inputItem.Portrait, siteItem.Id);
            if (convertPortrait == null) throw new NullReferenceException("Portrait failed to convert.");

            inputItem.Portrait.Id = convertPortrait.Id;
            string portraitSaveStatus = await Image.Save(site, inputItem.Portrait);
            if (portraitSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Image failed to save.");

            var saveItem = new Tables.dbo.Testimonial
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteItem.Id,
                WorkId = convertWork.Id,
                PortraitImageId = convertPortrait.Id,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress,
                Approved = inputItem.Approved
            };

            SaveStatus output = await dbWrite.Item(site, saveItem);
            if (output == SaveStatus.Failed) return output.ToString();

            var saveLocalizationFailed = false;
            foreach (Tables.Localization.Testimonial saveLocalization in inputItem.Localization.Select(localizedPage =>
                new Tables.Localization.Testimonial()))
            {
                saveLocalization.Id = saveItem.Id;
                SaveStatus saveStatus = await DatabaseOperations.Localization.Write.Testimonial.Item(site, saveLocalization);
                saveLocalizationFailed = saveStatus == SaveStatus.Failed;
                if (saveLocalizationFailed) break;
            }

            if (saveLocalizationFailed) output = SaveStatus.Failed;
            return output.ToString();

        }
    }
}
