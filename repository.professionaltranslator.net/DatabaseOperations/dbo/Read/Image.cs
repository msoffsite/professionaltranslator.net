using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Image : Base
    {
        internal static async Task<Tables.dbo.Image> DefaultPortfolio(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetImageDefaultPortfolio]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Image>(sda);
        }

        internal static async Task<Tables.dbo.Image> DefaultTestimonial(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetImageDefaultTestimonial]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Image>(sda);
        }

        internal static async Task<Tables.dbo.Image> Item(Guid? id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetImage]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Image>(sda);
        }

        internal static async Task<Tables.dbo.Image> Item(string site, string path)
        {
            await using var cmd = new SqlCommand("[dbo].[GetImageBySitePath]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Path", SqlDbType.NVarChar, 4440).Value = path;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Image>(sda);
        }

        internal static async Task<List<Tables.dbo.Image>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetImages]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Image>(sda);
        }
    }
}
