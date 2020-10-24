using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Image;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Image
    {
        internal static Tables.dbo.Image Convert(models.Image inputItem, Guid siteId)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Image
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                SiteId = siteId,
                Path = inputItem.Path
            };
            return output;
        }

        public static async Task<models.Image> Item(Guid id)
        {
            Tables.dbo.Image image = await dbRead.Image.Item(id);
            if (image == null) return null;
            var output = new models.Image
            {
                Id = image.Id,
                Path = image.Path
            };
            return output;
        }

        public static async Task<List<models.Image>> List(string site)
        {
            List<Tables.dbo.Image> list = await dbRead.Image.List(site);
            return list.Select(n => new models.Image
            {
                Id = n.Id,
                Path = n.Path
            }).ToList();
        }

        /// <summary>
        /// See https://stackoverflow.com/questions/39322085/how-to-save-iformfile-to-disk for admin save file.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="inputItem"></param>
        /// <returns></returns>
        public static async Task<string> Save(string site, models.Image inputItem)
        {
            if (inputItem == null)  throw new NullReferenceException("Image cannot be null.");
            if (string.IsNullOrEmpty(inputItem.Path)) throw new ArgumentNullException(nameof(inputItem.Path), "Path cannot be empty.");
            if (inputItem.Path.Length > 440) throw new ArgumentException("Path must be 440 characters or fewer.", nameof(inputItem.Path));
            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name. Cannot continue.");
            Tables.dbo.Image convertItem = Convert(inputItem, siteItem.Id);
            SaveStatus output = await dbWrite.Item(site, convertItem);
            return output.ToString();
        }
    }
}
