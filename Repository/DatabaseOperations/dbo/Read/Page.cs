using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Read
{
    internal class Page : Base
    {
        internal static async Task<Tables.dbo.Page> Item(string site, string name)
        {
            await using var cmd = new SqlCommand("[dbo].[GetPage]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Enum", SqlDbType.NVarChar, 20).Value = name;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Page>(sda);
        }

        internal static async Task<List<Tables.dbo.Page>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetPages]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Page>(sda);
        }
    }
}
