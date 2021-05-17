using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Work;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Work
    {
        public static async Task<int> PagingCount(string site, Display display)
        {
            bool show = display == Display.Approved;
            return await dbRead.Work.PagingCount(site, show);
        }

        public static async Task<int> PagingCountWithoutTestimonials(string site)
        {
            return await dbRead.Work.PagingCountWithoutTestimonials(site);
        }

        public static async Task<int> PagingCountWithTestimonials(string site)
        {
            return await dbRead.Work.PagingCountWithTestimonials(site);
        }

        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(ResultStatus.Failed, "Id must be a valid GUID.");
            }

            return await dbWrite.Delete(site, id.Value);
        }

        internal static Tables.dbo.Work Convert(models.Work inputItem, Guid siteId)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Work
            {
                Id = inputItem.Id,
                SiteId = siteId,
                CoverId = inputItem.Cover.Id,
                Title = inputItem.Title,
                Authors = inputItem.Authors,
                Href = inputItem.Href,
                DateCreated = inputItem.DateCreated,
                Display = inputItem.Display
            };
            return output;
        }

        public static async Task<models.Work> Item(Guid? id)
        {
            if (!id.HasValue) return null;
            Tables.dbo.Work item = await dbRead.Work.Item(id.Value);
            if (item == null) return null;
            var output = new models.Work
            {
                Id = item.Id,
                Cover = await Image.Item(item.CoverId),
                Title = item.Title,
                Authors = item.Authors,
                Href = item.Href,
                DateCreated = item.DateCreated,
                Display = item.Display
            };
            return output;
        }


        // ReSharper disable once UnusedMember.Local
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static async Task<Tables.dbo.Work> Item(string site, string title, string authors)
        {
            Tables.dbo.Work output = await dbRead.Work.Item(site, title, authors);
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

        public static async Task<List<models.Work>> List(string site, Display display)
        {
            bool show = display == Display.Approved;
            List<Task<models.Work>> taskList = await TaskList(site, show);
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

        public static async Task<List<models.Work>> List(string site, Display display, int pageIndex, int pageSize)
        {
            bool show = display == Display.Approved;
            List<Task<models.Work>> taskList = await TaskList(site, show, pageIndex, pageSize);
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

        public static async Task<List<models.Work>> ListWithoutTestimonials(string site, int pageIndex, int pageSize)
        {
            List<Task<models.Work>> taskList = await TaskListTestimonials(site, false, pageIndex, pageSize);
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

        public static async Task<List<models.Work>> ListWithTestimonials(string site, int pageIndex, int pageSize)
        {
            List<Task<models.Work>> taskList = await TaskListTestimonials(site, true, pageIndex, pageSize);
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

        public static async Task<bool> ShowNextForPaging(string site, Display display, int pageIndex, int pageSize)
        {
            bool show = display == Display.Approved;
            return await dbRead.Work.ShowNextForPaging(site, show, pageIndex, pageSize);
        }

        private static async Task<List<Task<models.Work>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site, approved);
            return Complete(list);
        }

        private static async Task<List<Task<models.Work>>> TaskList(string site, bool approved, int pageIndex, int pageSize)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Work>>();
            List<Tables.dbo.Work> list = await dbRead.Work.List(site, approved, pageIndex, pageSize);
            return Complete(list);
        }

        private static async Task<List<Task<models.Work>>> TaskListTestimonials(string site, bool with, int pageIndex, int pageSize)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Work>>();
            List<Tables.dbo.Work> list;
            if (!with)
            {
                list = await dbRead.Work.ListWithoutTestimonials(site, pageIndex, pageSize);
            }
            else
            {
                list = await dbRead.Work.ListWithTestimonials(site, pageIndex, pageSize);
            }
            
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
                Display = n.Display
            }).ToList();
        }

        /// <summary>
        /// Saves work and child items.
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site related to work.</param>
        /// <param name="inputItem">Work object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, models.Work inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Work cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Tables.dbo.Image convertImage = Image.Convert(inputItem.Cover, siteItem.Id);
            if (convertImage == null)
            {
                return new Result(ResultStatus.Failed, "Work must have a cover image.");
            }

            Result saveImageResult = await Image.Save(site, inputItem.Cover);
            if (saveImageResult.Status == ResultStatus.Failed)
            {
                return new Result(saveImageResult.Status, messages);
            }

            Rules.StringRequiredMaxLength(inputItem.Title, "Title", 100, ref messages);
            Rules.StringRequiredMaxLength(inputItem.Authors, "Authors", 255, ref messages);
            if (Rules.StringRequiredMaxLength(inputItem.Href, "Href", 2048, ref messages) == Rules.Passed.Yes)
            {
                Rules.ValidateUrl(inputItem.Href, "Href", ref messages);
            }

            if (messages.Any()) return new Result(ResultStatus.Failed, messages);

            Tables.dbo.Work convertedWork = Convert(inputItem, siteItem.Id);
            if (convertedWork == null) return new Result(ResultStatus.Failed, "Could not convert Work model to table.");

            Result output = await dbWrite.Item(site, convertedWork);
            if (output.Status == ResultStatus.PartialSuccess || output.Status == ResultStatus.Succeeded)
            {
                output.ReturnId = inputItem.Id;
            }
            return output;
        }
    }
}
