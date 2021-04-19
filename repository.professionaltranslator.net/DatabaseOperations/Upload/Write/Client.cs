using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Write
{
    internal class Client : Base
    {
        internal static async Task<Result> Item(Tables.Upload.Client item)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Upload].[SaveClient]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@ClientId", SqlDbType.UniqueIdentifier).Value = item.ClientId;
                cmd.Parameters.Add("@OriginalFilename", SqlDbType.NVarChar, 256).Value = item.OriginalFilename;
                cmd.Parameters.Add("@GeneratedFilename", SqlDbType.NVarChar, 45).Value = item.GeneratedFilename;

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