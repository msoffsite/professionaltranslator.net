using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Read
{
    internal class Image : Base
    {
        internal static Models.Image Item(Guid? id)
        {
            using var cmd = new SqlCommand("[dbo].[GetImage]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Models.Image>(sda);
        }

        internal static List<Models.Image> List()
        {
            using var cmd = new SqlCommand("[dbo].[GetImages]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Models.Image>(sda);
        }
    }
}
