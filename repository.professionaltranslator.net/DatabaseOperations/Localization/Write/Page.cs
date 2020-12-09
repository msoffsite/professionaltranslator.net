using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Write
{
    internal class Page : Base
    {
        internal static async Task<Result> Item(string site, Tables.Localization.Page item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Localization].[SavePage]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@PageId", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = item.Lcid;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = item.Title;
                cmd.Parameters.Add("@Html", SqlDbType.NVarChar, -1).Value = item.Html;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "Localization.Page");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(saveStatus, messages);
        }
    }
}
