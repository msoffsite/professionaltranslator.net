using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Work : Base
    {
        internal static async Task<Tables.dbo.Work> Item(Guid id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWork]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Work>(sda);
        }

        internal static async Task<List<Tables.dbo.Work>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorks]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Work>(sda);
        }

        internal static async Task<List<Tables.dbo.Work>> List(string site, bool display)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorksForDisplay]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Display", SqlDbType.UniqueIdentifier).Value = display;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Work>(sda);
        }
    }
}
