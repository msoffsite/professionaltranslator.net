using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Site;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.dbo.Write.Site;

namespace professionaltranslator.net.Repository
{
    public class Site
    {
        public static async Task<Models.Site> Item(string enumerator)
        {
            Tables.dbo.Site item = await dbRead.Item(enumerator);
            if (item == null) return null;
            var output = new Models.Site
            {
                Id = item.Id,
                Name = item.Name
            };
            return output;
        }

        public static async Task<List<Models.Site>> List()
        {
            List<Tables.dbo.Site> list = await dbRead.List();
            return list.Select(n => new Models.Site
            {
                Id = n.Id,
                Name = n.Name
            }).ToList();
        }

        public static async Task<string> Save(Models.Site item)
        {
            if (item == null) throw new NullReferenceException("Site cannot be null.");
            if (string.IsNullOrEmpty(item.Name)) throw new ArgumentNullException(nameof(item.Name), "Name cannot be empty.");
            if (item.Name.Length > 25) throw new ArgumentException("Name must be 25 characters or fewer.", nameof(item.Name));
            SaveStatus output = await dbWrite.Item(item);
            return output.ToString();
        }
    }
}
