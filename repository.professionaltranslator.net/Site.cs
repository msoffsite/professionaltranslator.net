using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read.Site;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Site;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Site
    {
        internal static Tables.dbo.Site Convert(models.Site inputItem)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Site
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                Name = inputItem.Name
            };
            return output;
        }

        public static async Task<models.Site> Item(string enumerator)
        {
            Tables.dbo.Site item = await dbRead.Item(enumerator);
            if (item == null) return null;
            var output = new models.Site
            {
                Id = item.Id,
                Name = item.Name
            };
            return output;
        }

        public static async Task<List<models.Site>> List()
        {
            List<Tables.dbo.Site> list = await dbRead.List();
            return list.Select(n => new models.Site
            {
                Id = n.Id,
                Name = n.Name
            }).ToList();
        }

        public static async Task<string> Save(models.Site item)
        {
            if (item == null) throw new NullReferenceException("Site cannot be null.");
            if (string.IsNullOrEmpty(item.Name)) throw new ArgumentNullException(nameof(item.Name), "Name cannot be empty.");
            if (item.Name.Length > 25) throw new ArgumentException("Name must be 25 characters or fewer.", nameof(item.Name));
            Tables.dbo.Site saveItem = Convert(item);
            SaveStatus output = await dbWrite.Item(saveItem);
            return output.ToString();
        }
    }
}
