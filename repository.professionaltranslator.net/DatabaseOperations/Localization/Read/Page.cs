using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read
{
    internal class Page : Base
    {
        internal static async Task<List<Tables.Localization.Page>> List(Guid pageId)
        {
            await using var cmd = new SqlCommand("[dbo].[GetLocalizedPages]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@PageId", SqlDbType.UniqueIdentifier).Value = pageId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Localization.Page>(sda);
        }
    }
}
