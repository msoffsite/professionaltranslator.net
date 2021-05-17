using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read.Site;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Site;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Site
    {
        internal static Tables.dbo.Site Convert(models.Site inputItem)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Site
            {
                Id = inputItem.Id,
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

        /// <summary>
        /// Saves site item.
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="item">Site object</param>
        /// <returns>Result with save status and messages if error occurs. Does not return an identifier.</returns>
        public static async Task<Result> Save(models.Site item)
        {
            var messages = new List<string>();

            if (item == null)
            {
                return new Result(ResultStatus.Failed, "Site cannot be null.");
            }

            Rules.StringRequiredMaxLength(item.Name, "Name", 25, ref messages);

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            Tables.dbo.Site convertedSite = Convert(item);
            if (convertedSite == null)
            {
                return new Result(ResultStatus.Failed, "Could not convert Site model to table.");
            }

            Result saveResult = await dbWrite.Item(convertedSite);
            return saveResult;
        }
    }
}
