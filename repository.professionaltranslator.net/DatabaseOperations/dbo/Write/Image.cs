using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Image : Base
    {
        internal static async Task<Result> Delete(string site, Guid id)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[DeleteImage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Image");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(saveStatus, messages);
        }

        /// <summary>
        /// Saves image item.
        /// </summary>
        /// <param name="site">Name of site.</param>
        /// <param name="item">Image item.</param>
        /// <returns>Result with save status and messages if error occurs. Does not return an identifier.</returns>
        internal static async Task<Result> Item(string site, Tables.dbo.Image item)
        {
            var messages = new List<string>();
            var saveStatus = SaveStatus.Undetermined;

            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveImage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = item.SiteId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@Path", SqlDbType.NVarChar, 440).Value = item.Path;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Image");
                messages.Add(ex.Message);
                saveStatus = SaveStatus.Failed;
            }

            return new Result(saveStatus, messages);
        }
    }
}
