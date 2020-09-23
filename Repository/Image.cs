using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Image;

namespace professionaltranslator.net.Repository
{
    internal class Image
    {
        internal static Task<Models.Image> Get(Guid? id)
        {
            try
            {
                return dbRead.Item(id);
            }
            catch
            {
                return null;
            }
        }

        internal static async Task<List<Models.Image>> Get()
        {
            try
            {
                return await dbRead.List();
            }
            catch
            {
                return new List<Models.Image>();
            }
        }
    }
}
