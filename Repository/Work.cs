using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Work;

namespace professionaltranslator.net.Repository
{
    public class Work
    {
        public static async Task<Models.Work> Item(Guid id)
        {
            Tables.dbo.Work item = await dbRead.Item(id);
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
            List<Tables.dbo.Work> list = await dbRead.List(site);
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
            List<Tables.dbo.Work> list = await dbRead.List(site, approved);
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


    }
}
