using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Localized
{
    internal class Page : Base
    {
        internal static async Task<Models.Localized.Page> Item(string site, string name, string culture)
        {
            await using var cmd = new SqlCommand("[dbo].[GetLocalizedPage]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Enum", SqlDbType.NVarChar, 20).Value = name;
            cmd.Parameters.Add("@Culture", SqlDbType.NVarChar, 3).Value = culture;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Models.Localized.Page>(sda);
        }
    }
}
