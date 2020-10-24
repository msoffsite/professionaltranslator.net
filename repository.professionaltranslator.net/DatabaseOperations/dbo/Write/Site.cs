using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Site : Base
    {
        internal static async Task<SaveStatus> Item(Tables.dbo.Site inputItem)
        {
            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveSite]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = inputItem.Id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 25).Value = inputItem.Name;
                //await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(inputItem.Name, ex, "dbo.Site");
                return SaveStatus.Failed;
            }
        }
    }
}
