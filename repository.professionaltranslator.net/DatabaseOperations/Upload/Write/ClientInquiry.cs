using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Write
{
    internal class ClientInquiry
    {
        internal static async Task<Result> Item(Guid uploadId, Guid inquiryId)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Upload].[SaveClientInquiry]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@UploadId", SqlDbType.UniqueIdentifier).Value = uploadId;
                cmd.Parameters.Add("@InquiryId", SqlDbType.UniqueIdentifier).Value = inquiryId;

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
