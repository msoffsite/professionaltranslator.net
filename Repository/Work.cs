using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.dbo.Write.Work;

namespace professionaltranslator.net.Repository
{
    public class Work
    {
        public static async Task<Models.Work> Item(Guid id)
        {
            Tables.dbo.Work item = await dbRead.Work.Item(id);
            if (item == null) return null;
            var output = new Models.Work
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

        public static async Task<List<Models.Work>> List(string site)
        {
            List<Task<Models.Work>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<Models.Work>();
            var output = new List<Models.Work>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                Models.Work item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<Models.Work>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<Models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site);
            return Complete(list);
        }

        public static async Task<List<Models.Work>> List(string site, bool approved)
        {
            List<Task<Models.Work>> taskList = await TaskList(site, approved);
            if (taskList.Count == 0) return new List<Models.Work>();
            var output = new List<Models.Work>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                Models.Work item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }
            return output;
        }

        private static async Task<List<Task<Models.Work>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<Models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site, approved);
            return Complete(list);
        }

        private static List<Task<Models.Work>> Complete(IEnumerable<Tables.dbo.Work> inputList)
        {
            return inputList.Select(async n => new Models.Work
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

        public static async Task<string> Save(string site, Models.Work work)
        {
            if (work == null) throw new NullReferenceException("Image cannot be null.");
            if (string.IsNullOrEmpty(work.Title)) throw new ArgumentNullException(nameof(work.Title), "Title cannot be empty.");
            if (string.IsNullOrEmpty(work.Authors)) throw new ArgumentNullException(nameof(work.Authors), "Authors cannot be empty.");
            if (string.IsNullOrEmpty(work.Href)) throw new ArgumentNullException(nameof(work.Href), "Href cannot be empty.");
            if (string.IsNullOrEmpty(work.TestimonialLink)) throw new ArgumentNullException(nameof(work.TestimonialLink), "Testimonial link cannot be empty.");
            if (work.Title.Length > 100) throw new ArgumentException("Title must be 100 characters or fewer.", nameof(work.Title));
            if (work.Authors.Length > 255) throw new ArgumentException("Authors must be 255 characters or fewer.", nameof(work.Authors));
            if (work.Href.Length > 2048) throw new ArgumentException("Href must be 2048 characters or fewer.", nameof(work.Href));
            if (work.TestimonialLink.Length > 100) throw new ArgumentException("Testimonial link must be 100 characters or fewer.", nameof(work.TestimonialLink));
            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name. Cannot continue.");
            string imageSaveStatus = await Image.Save(site, work.Cover);
            if (imageSaveStatus == SaveStatus.Failed.ToString()) throw new Exception("Cover image failed to save. Cannot continue.");
            SaveStatus output = await dbWrite.Item(siteItem.Id, work);
            return output.ToString();
        }
    }
}
