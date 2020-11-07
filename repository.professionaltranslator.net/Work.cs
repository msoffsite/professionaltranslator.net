using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Professionaltranslator.Net;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Work;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Work
    {
        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(SaveStatus.Failed, "Id must be a valid GUID.");
            }

            return await dbWrite.Delete(site, id.Value);
        }

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

        public static async Task<Result> Save(string site, models.Work inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(SaveStatus.Failed, "Work cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(SaveStatus.Failed, "No site was found with that name.");
            }

            Tables.dbo.Image convertImage = Image.Convert(inputItem.Cover, siteItem.Id);
            if (convertImage == null)
            {
                return new Result(SaveStatus.Failed, "Work must have a cover image.");
            }

            Result saveImageResult = await Image.Save(site, inputItem.Cover);
            if (saveImageResult.Status == SaveStatus.Failed)
            {
                return new Result(saveImageResult.Status, messages);
            }

            inputItem.Cover.Id = convertImage.Id;

            if (string.IsNullOrEmpty(inputItem.Title))
            {
                messages.Add("Title cannot be empty.");
            }
            else if (inputItem.Title.Length > 100)
            {
                messages.Add("Title must be 100 characters or fewer.");
            }

            if (string.IsNullOrEmpty(inputItem.Authors))
            {
                messages.Add("Authors cannot be empty.");
            }
            else if (inputItem.Authors.Length > 255)
            {
                messages.Add("Authors must be 255 characters or fewer.");
            }

            if (string.IsNullOrEmpty(inputItem.Href))
            {
                messages.Add("Href cannot be empty.");
            }
            else if (inputItem.Href.Length > 2048)
            {
                messages.Add("Href must be 2048 characters or fewer.");
            }

            if (string.IsNullOrEmpty(inputItem.TestimonialLink))
            {
                messages.Add("Testimonial link cannot be empty.");
            }
            else if (inputItem.TestimonialLink.Length > 100)
            {
                messages.Add("Testimonial link must be 100 characters or fewer.");
            }

            if (messages.Any()) return new Result(SaveStatus.Failed, messages);

            Tables.dbo.Work convertedWork = Convert(inputItem, siteItem.Id);
            if (convertedWork == null) return new Result(SaveStatus.Failed, "Could not convert Work model to table.");
            
            Result output = await dbWrite.Item(site, convertedWork);
            return output;
        }
    }
}
