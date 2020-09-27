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
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<string> Save(string site, Models.Image image)
        {
            if (image == null)  throw new NullReferenceException("Image cannot be null.");
            if (string.IsNullOrEmpty(image.Path)) throw new ArgumentNullException(nameof(image), "Image path cannot be empty.");
            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null) throw new NullReferenceException("No site was found with that name.");
            SaveStatus output = await dbWrite.Item(siteItem.Id, image);
            return output.ToString();
        }
    }
}
