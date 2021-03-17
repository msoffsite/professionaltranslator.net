using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Work : Base
    {
        internal static async Task<Result> Delete(string site, Guid id)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[DeleteWork]", new Base().SqlConnection)
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
                await Exception.Save(site, ex, "dbo.Work");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(saveStatus, messages);
        }

        internal static async Task<Result> Item(string site, Tables.dbo.Work item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveWork]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = item.SiteId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@CoverId", SqlDbType.UniqueIdentifier).Value = item.CoverId;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = item.Title;
                cmd.Parameters.Add("@Authors", SqlDbType.NVarChar, 255).Value = item.Authors;
                cmd.Parameters.Add("@Href", SqlDbType.NVarChar, 2048).Value = item.Href;
                cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = item.Display;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Work");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }
            return new Result(saveStatus, messages);
        }
    }
}
