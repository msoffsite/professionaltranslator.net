using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Write
{
    internal class Image : Base
    {
        internal static async Task<SaveStatus> Item(Guid siteId, Models.Image image)
        {
            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveImage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = siteId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = image.Id ?? Guid.NewGuid();
                cmd.Parameters.Add("@Path", SqlDbType.NVarChar, 440).Value = image.Path;
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
