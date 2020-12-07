using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read
{
    internal class PageHeader : Base
    {
        internal static async Task<List<Tables.Localization.PageHeader>> List(Guid pageId)
        {
            await using var cmd = new SqlCommand("[dbo].[GetLocalizedPageHeaders]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@PageId", SqlDbType.UniqueIdentifier).Value = pageId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Localization.PageHeader>(sda);
        }
    }
}