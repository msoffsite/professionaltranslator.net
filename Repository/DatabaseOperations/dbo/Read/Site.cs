﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.dbo.Read
{
    internal class Site : Base
    {
        internal static async Task<Tables.dbo.Site> Item(string enumerator)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSite]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Enum", SqlDbType.NVarChar, 25).Value = enumerator;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Site>(sda);
        }
    }
}
