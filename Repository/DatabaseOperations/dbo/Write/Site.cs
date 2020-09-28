using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Write
{
    internal class Site : Base
    {
        internal static async Task<SaveStatus> Item(Tables.dbo.Site site)
        {
            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveSite]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = site.Id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 25).Value = site.Name;
                //await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return SaveStatus.Succeeded;
            }
            catch (Exception)
            {
                return SaveStatus.Failed;
            }
        }
    }
}
