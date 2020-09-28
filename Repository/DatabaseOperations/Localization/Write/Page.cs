﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace professionaltranslator.net.Repository.DatabaseOperations.Localization.Write
{
    internal class Page : Base
    {
        internal static async Task<SaveStatus> Item(Tables.Localization.Page item)
        {
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
                //await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return SaveStatus.Succeeded;
            }
            catch (Exception)
            {
                return SaveStatus.Failed;
            }
        }
    }
}
