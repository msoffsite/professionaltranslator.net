using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Read.Client;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Client;
using Model = Models.ProfessionalTranslator.Net.Upload.Client;
using Table = Repository.ProfessionalTranslator.Net.Tables.Upload.Client;

namespace Repository.ProfessionalTranslator.Net.Upload
{
    public class Client
    {
        public static async Task<Model> Item(Guid? clientId, string originalFilename)
        {
            if (!clientId.HasValue || string.IsNullOrEmpty(originalFilename)) return null;
            Table item = await dbRead.Item(clientId.Value, originalFilename);
            return item == null ? null : Item(item);
        }

        private static Model Item(Table item)
        {
            try
            {
                if (item == null) throw new ArgumentNullException(nameof(item));

                var output = new Model
                {
                    Id = item.Id,
                    GeneratedFilename = item.GeneratedFilename,
                    OriginalFilename = item.OriginalFilename
                };
                return output;
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }
    }
}
