﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Testimonial : Base
    {
        internal static async Task<Tables.dbo.Testimonial> Item(Guid id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetTestimonial]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Testimonial>(sda);
        }

        internal static async Task<Tables.dbo.Testimonial> Item(Guid siteId, Guid workId)
        {
            await using var cmd = new SqlCommand("[dbo].[GetTestimonialBySiteIdWorkId]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = siteId;
            cmd.Parameters.Add("@WorkId", SqlDbType.UniqueIdentifier).Value = workId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Testimonial>(sda);

        }
        internal static async Task<List<Tables.dbo.Testimonial>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetTestimonials]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Testimonial>(sda);
        }

        internal static async Task<List<Tables.dbo.Testimonial>> List(string site, bool approved)
        {
            await using var cmd = new SqlCommand("[dbo].[GetTestimonialsForSiteApproved]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Approved", SqlDbType.UniqueIdentifier).Value = approved;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Testimonial>(sda);
        }
    }
}
