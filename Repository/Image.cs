using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Image;

namespace professionaltranslator.net.Repository
{
    public class Image
    {
        public static async Task<Models.Image> Get(Guid? id)
        {
            if (!id.HasValue) return null;
            Tables.dbo.Image image = await dbRead.Item(id);
            if (image == null) return null;
            var output = new Models.Image
            {
                Id = image.Id,
                Path = image.Path
            };
            return output;
        }

        public static async Task<List<Models.Image>> Get()
        {
            List<Tables.dbo.Image> list = await dbRead.List();
            return list.Select(n => new Models.Image
            {
                Id = n.Id,
                Path = n.Path
            }).ToList();
        }
    }
}
