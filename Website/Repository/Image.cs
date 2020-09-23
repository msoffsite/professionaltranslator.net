using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace professionaltranslator.net.Repository
{
    internal class Image : Base
    {
        internal static Models.Image Get(Guid? id)
        {
            if (!id.HasValue) return null;

            var connectionStrings = new ConnectionStrings();
            using var con = new SqlConnection(connectionStrings.SqlServer);
            using var cmd = new SqlCommand(StoredProcedures.Dbo.Read.GetImageById, con);
            using var sda = new SqlDataAdapter(cmd);
            return Models.Image.Get(sda);
        }
    }
}
