using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read;
using dbWrite = professionaltranslator.net.Repository.DatabaseOperations.dbo.Write.Image;

namespace professionaltranslator.net.Repository
{
    public class Image
    {
        internal static Tables.dbo.Image Convert(Models.Image inputItem, Guid siteId)
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

        public static async Task<Models.Image> Item(Guid id)
        {
            Tables.dbo.Image image = await dbRead.Image.Item(id);
            if (image == null) return null;
            var output = new Models.Image
            {
                Id = image.Id,
                Path = image.Path
            };
            return output;
        }

        public static async Task<List<Models.Image>> List(string site)
        {
            List<Tables.dbo.Image> list = await dbRead.Image.List(site);
            return list.Select(n => new Models.Image
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
        public static async Task<string> Save(string site, Models.Image inputItem)
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
