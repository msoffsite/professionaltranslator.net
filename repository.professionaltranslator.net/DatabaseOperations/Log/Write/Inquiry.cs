using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Write
{
    internal class Inquiry : Base
    {
        internal static async Task<Result> Item(string site, Tables.Log.Inquiry item)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Log].[SaveInquiry]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 20).Value = site;
                cmd.Parameters.Add("@ClientId", SqlDbType.UniqueIdentifier).Value = item.ClientId;
                cmd.Parameters.Add("@TranslationType", SqlDbType.NVarChar, 25).Value = item.TranslationType;
                cmd.Parameters.Add("@TranslationDirection", SqlDbType.NVarChar, 25).Value = item.TranslationDirection;
                cmd.Parameters.Add("@SubjectMatter", SqlDbType.NVarChar, 50).Value = item.SubjectMatter;
                cmd.Parameters.Add("@WordCount", SqlDbType.Int).Value = item.WordCount;
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = item.Message;

                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                resultStatus = ResultStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                resultStatus = ResultStatus.Failed;
                messages.Add(ex.Message);
            }
            return new Result(resultStatus, messages);
        }
    }
}
