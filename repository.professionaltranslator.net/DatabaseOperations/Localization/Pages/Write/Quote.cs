using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Pages.Write
{
    internal class Page : Base
    {
        internal static async Task<Result> Item(string site, Tables.Localization.Pages.Quote item)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Localization].[SavePageQuote]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@PageId", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = item.Lcid;
                cmd.Parameters.Add("@Text", SqlDbType.NVarChar, 150).Value = item.Text;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                resultStatus = ResultStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "Localization.Pages.Write");
                resultStatus = ResultStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(resultStatus, messages);
        }
    }
}
