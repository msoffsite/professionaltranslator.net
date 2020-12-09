using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write
{
    internal class Exception : Base
    {
        internal static async Task<Result> Item(string site, Tables.Log.Exception item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Log].[SaveException]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 20).Value = site;
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = item.Message;
                cmd.Parameters.Add("@Stacktrace", SqlDbType.NVarChar, -1).Value = item.Stacktrace;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = item.Type;
                cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 50).Value = item.Class;
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
