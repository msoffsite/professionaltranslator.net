using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using dbRead = professionaltranslator.net.Repository.StoredProcedures.Dbo.Read;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository
{
    internal class Image : Object
    {
        internal static Models.Image Get(Guid? id)
        {
            if (!id.HasValue) return null;

            var connectionStrings = new ConnectionStrings();
            using var con = new SqlConnection(connectionStrings.SqlServer);
            using var cmd = new SqlCommand(dbRead.Image.ItemById, con);
            using var sda = new SqlDataAdapter(cmd);
            return GetItem<Models.Image>(sda);
        }
    }
}
