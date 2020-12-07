using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Site : Base
    {
        internal static async Task<Result> Item(Tables.dbo.Site inputItem)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveSite]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = inputItem.Id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 25).Value = inputItem.Name;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(inputItem.Name, ex, "dbo.Site");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }
            return new Result(saveStatus, messages);
        }
    }
}
