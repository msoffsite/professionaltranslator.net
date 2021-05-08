using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Client : Base
    {
        internal static async Task<Tables.dbo.Client> Item(Guid id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetClient]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Client>(sda);
        }

        internal static async Task<Tables.dbo.Client> Item(string emailAddress)
        {
            await using var cmd = new SqlCommand("[dbo].[GetClientByEmailAddress]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 256).Value = emailAddress;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Client>(sda);
        }

        internal static async Task<List<Tables.dbo.Client>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetClients]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Client>(sda);
        }
    }
}
