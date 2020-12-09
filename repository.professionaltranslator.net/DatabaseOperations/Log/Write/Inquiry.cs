using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write
{
    internal class Inquiry : Base
    {
        internal static async Task<Result> Item(string site, Tables.Log.Inquiry item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Log].[SaveInquiry]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 20).Value = site;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = item.Name;
                cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 256).Value = item.EmailAddress;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 256).Value = item.Title;
                cmd.Parameters.Add("@TranslationType", SqlDbType.NVarChar, 25).Value = item.TranslationType;
                cmd.Parameters.Add("@Genre", SqlDbType.NVarChar, 25).Value = item.Genre;
                cmd.Parameters.Add("@WordCount", SqlDbType.Int).Value = item.WordCount;
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = item.Message;

                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }
            return new Result(saveStatus, messages);
        }
    }
}
