using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Testimonial;
using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.Localization.Read.Testimonial;

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
            List<Tables.dbo.Testimonial> list = await dbRead.List(site);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site);
            return Merge(list, localization);
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
            List<Tables.dbo.Testimonial> list = await dbRead.List(site, approved);
            List<Tables.Localization.Testimonial> localization = await dbLocalizedRead.List(site, approved);
            return Merge(list, localization);
        }

        private static List<Task<Models.Testimonial>> Merge(IEnumerable<Tables.dbo.Testimonial> testimonialList,
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
                Display = n.Display,
                Localization = localizedList.Where(x => x.Id == n.Id).Select(t => new Models.Localized.Testimonial
                {
                    Lcid = t.Lcid,
                    Html = t.Html
                }).ToList()
            }).ToList();
        }
    }
}
