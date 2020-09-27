using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = professionaltranslator.net.Repository.Conversions.Object;

namespace professionaltranslator.net.Repository.DatabaseOperations.Localization.Read
{
    internal class Testimonial : Base
    {
        internal static async Task<List<Tables.Localization.Testimonial>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetLocalizedTestimonials]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Localization.Testimonial>(sda);
        }

        internal static async Task<List<Tables.Localization.Testimonial>> List(string site, bool approved)
        {
            await using var cmd = new SqlCommand("[dbo].[GetLocalizedTestimonialsForSiteApproved]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Approved", SqlDbType.UniqueIdentifier).Value = approved;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Localization.Testimonial>(sda);
        }
    }
}
