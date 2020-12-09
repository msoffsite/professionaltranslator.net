using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Log.Read
{
    internal class Inquiry : Base
    {
        internal static async Task<Tables.Log.Inquiry> Item(Guid? id)
        {
            await using var cmd = new SqlCommand("[Log].[GetInquiry]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Conversions.Object.GetItem<Tables.Log.Inquiry>(sda);
        }

        internal static async Task<List<Tables.Log.Inquiry>> List(string site)
        {
            await using var cmd = new SqlCommand("[Log].[GetInquiries]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Log.Inquiry>(sda);
        }
    }
}
