using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Image;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Image
    {
        public static async Task<models.Image> DefaultPortfolio(string site)
        {
            Tables.dbo.Image image = await dbRead.Image.DefaultPortfolio(site);
            return Item(image);
        }

        public static async Task<models.Image> DefaultTestimonial(string site)
        {
            Tables.dbo.Image image = await dbRead.Image.DefaultTestimonial(site);
            return Item(image);
        }

        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(ResultStatus.Failed, "Id must be a valid GUID.");
            }

            return await dbWrite.Delete(site, id.Value);
        }

        internal static Tables.dbo.Image Convert(models.Image inputItem, Guid siteId)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Image
            {
                Id = inputItem.Id,
                SiteId = siteId,
                Path = inputItem.Path
            };
            return output;
        }

        public static async Task<models.Image> Item(Guid id)
        {
            Tables.dbo.Image image = await dbRead.Image.Item(id);
            return Item(image);
        }

        public static async Task<models.Image> Item(string site, string path)
        {
            if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(path))) return null;
            Tables.dbo.Image image = await dbRead.Image.Item(site, path);
            return Item(image);
        }

        private static models.Image Item(Tables.dbo.Image inputItem)
        {
            if (inputItem == null) return null;
            var output = new models.Image
            {
                Id = inputItem.Id,
                Path = inputItem.Path
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
        /// Save image. 
        /// </summary>
        /// /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site for image.</param>
        /// <param name="inputItem">Image object.</param>
        /// <note>See https://stackoverflow.com/questions/39322085/how-to-save-iformfile-to-disk for admin save file.</note>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, models.Image inputItem)
        {
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Image cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Rules.StringRequiredMaxLength(inputItem.Path, "Path", 440, ref messages);

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            Tables.dbo.Image convertedImage = Convert(inputItem, siteItem.Id);
            if (convertedImage == null)
            {
                return new Result(ResultStatus.Failed, "Could not convert Image model to table.");
            }

            Result saveImageResult = await dbWrite.Item(site, convertedImage);
            if (saveImageResult.Status == ResultStatus.PartialSuccess || saveImageResult.Status == ResultStatus.Succeeded)
            {
                saveImageResult.ReturnId = inputItem.Id;
            }

            return saveImageResult;
        }
    }
}
