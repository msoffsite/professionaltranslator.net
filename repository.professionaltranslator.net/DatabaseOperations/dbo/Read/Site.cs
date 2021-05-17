using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net.Conversions;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Site : Base
    {
        internal static async Task<Tables.dbo.Site> Item(string enumerator)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSite]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Enum", SqlDbType.NVarChar, 25).Value = enumerator;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Site>(sda);
        }

        internal static async Task<List<Tables.dbo.Site>> List()
        {
            await using var cmd = new SqlCommand("[dbo].[GetSites]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Site>(sda);
        }
    }
}
