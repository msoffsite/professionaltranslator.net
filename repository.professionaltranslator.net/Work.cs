using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Work;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Work
    {
        internal static Tables.dbo.Work Convert(models.Work inputItem, Guid siteId)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Work
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteId,
                CoverId = inputItem.Cover.Id ?? throw new ArgumentNullException(nameof(inputItem.Cover.Id), "Cover must have an identifier."),
                Title = inputItem.Title,
                Authors = inputItem.Authors,
                Href = inputItem.Href,
                DateCreated = inputItem.DateCreated,
                Display = inputItem.Display,
                TestimonialLink = inputItem.TestimonialLink
            };
            return output;
        }

        public static async Task<models.Work> Item(Guid id)
        {
            Tables.dbo.Work item = await dbRead.Work.Item(id);
            if (item == null) return null;
            var output = new models.Work
            {
                Id = item.Id,
                Cover = await Image.Item(item.CoverId),
                Title = item.Title,
                Authors = item.Authors,
                Href = item.Href,
                DateCreated = item.DateCreated,
                Display = item.Display,
                TestimonialLink = item.TestimonialLink
            };
            return output;
        }

        public static async Task<List<models.Work>> List(string site)
        {
            List<Task<models.Work>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<models.Work>();
            var output = new List<models.Work>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Work item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<models.Work>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site);
            return Complete(list);
        }

        public static async Task<List<models.Work>> List(string site, bool approved)
        {
            List<Task<models.Work>> taskList = await TaskList(site, approved);
            if (taskList.Count == 0) return new List<models.Work>();
            var output = new List<models.Work>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Work item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<models.Work>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site, approved);
            return Complete(list);
        }

        private static List<Task<models.Work>> Complete(IEnumerable<Tables.dbo.Work> inputList)
        {
            return inputList.Select(async n => new models.Work
            {
                Id = n.Id,
                Cover = await Image.Item(n.CoverId),
                Title = n.Title,
                Authors = n.Authors,
                Href = n.Href,
                DateCreated = n.DateCreated,
                Display = n.Display,
                TestimonialLink = n.TestimonialLink
            }).ToList();
        }

        public static async Task<string> Save(string site, models.Work inputItem)
        {
            if (inputItem == null) throw new NullReferenceException("Work cannot be null.");
            if (string.IsNullOrEmpty(inputItem.Title)) throw new ArgumentNullException(nameof(inputItem.Title), "Title cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Authors)) throw new ArgumentNullException(nameof(inputItem.Authors), "Authors cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.Href)) throw new ArgumentNullException(nameof(inputItem.Href), "Href cannot be empty.");
            if (string.IsNullOrEmpty(inputItem.TestimonialLink)) throw new ArgumentNullException(nameof(inputItem.TestimonialLink), "Testimonial link cannot be empty.");
            if (inputItem.Title.Length > 100) throw new ArgumentException("Title must be 100 characters or fewer.", nameof(inputItem.Title));
            if (inputItem.Authors.Length > 255) throw new ArgumentException("Authors must be 255 characters or fewer.", nameof(inputItem.Authors));
            if (inputItem.Href.Length > 2048) throw new ArgumentException("Href must be 2048 characters or fewer.", nameof(inputItem.Href));
            if (inputItem.TestimonialLink.Length > 100) throw new ArgumentException("Testimonial link must be 100 characters or fewer.", nameof(inputItem.TestimonialLink));

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name. Cannot continue.");

            Tables.dbo.Image saveImage = Image.Convert(inputItem.Cover, siteItem.Id);
            if (saveImage == null) throw new NullReferenceException("Work must have a cover image.");
            inputItem.Cover.Id = saveImage.Id;
            string imageSaveStatus = await Image.Save(site, inputItem.Cover);
            if (imageSaveStatus == SaveStatus.Failed.ToString()) throw new System.Exception("Image failed to save.");

            Tables.dbo.Work convertItem = Convert(inputItem, siteItem.Id);
            SaveStatus output = await dbWrite.Item(site, convertItem);
            return output.ToString();
        }
    }
}
