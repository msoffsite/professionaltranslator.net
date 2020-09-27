using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Write
{
    internal class Work : Base
    {
        internal static async Task<SaveStatus> Item(Guid siteId, Models.Work work)
        {
            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveImage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = siteId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = work.Id ?? Guid.NewGuid();
                cmd.Parameters.Add("@CoverId", SqlDbType.UniqueIdentifier).Value = work.Cover.Id;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = work.Title;
                cmd.Parameters.Add("@Authors", SqlDbType.NVarChar, 255).Value = work.Authors;
                cmd.Parameters.Add("@Href", SqlDbType.NVarChar, 2048).Value = work.Href;
                cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = work.Display;
                cmd.Parameters.Add("@TestimonialLink", SqlDbType.NVarChar, 258).Value = work.TestimonialLink;
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
