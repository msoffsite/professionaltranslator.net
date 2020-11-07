using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Page : Base
    {
        internal static async Task<Result> Delete(string site, Guid id)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[DeletePage]", new Base().SqlConnection)
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
                await Exception.Save(site, ex, "dbo.Page");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(saveStatus, messages);
        }

        internal static async Task<Result> Item(string site, Tables.dbo.Page item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[SavePage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = item.SiteId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 20).Value = item.Name;
                cmd.Parameters.Add("@IsService", SqlDbType.Bit).Value = item.IsService;
                cmd.Parameters.Add("@CanHaveImage", SqlDbType.Bit).Value = item.CanHaveImage;
                cmd.Parameters.Add("@ImageId", SqlDbType.UniqueIdentifier).Value = item.ImageId;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Page");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(saveStatus, messages);
        }
    }
}
