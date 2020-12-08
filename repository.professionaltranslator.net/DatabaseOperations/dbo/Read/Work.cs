using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net.Conversions;
using Nullable = Repository.ProfessionalTranslator.Net.Conversions.Nullable;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Work : Base
    {
        internal static async Task<Tables.dbo.Work> Item(Guid id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWork]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Work>(sda);
        }

        internal static async Task<Tables.dbo.Work> Item(string site, string title, string authors)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorkBySiteTitleAuthors]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
            cmd.Parameters.Add("@Authors", SqlDbType.NVarChar, 255).Value = authors;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Work>(sda);
        }

        internal static async Task<List<Tables.dbo.Work>> List(string site)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorks]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Work>(sda);
        }

        internal static async Task<List<Tables.dbo.Work>> List(string site, bool display)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorksForDisplay]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = display;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Work>(sda);
        }

        internal static async Task<List<Tables.dbo.Work>> List(string site, bool display, int pageIndex, int pageSize)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorksPagingForDisplay]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = display;
            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Work>(sda);
        }

        internal static async Task<bool> ShowNextForPaging(string site, bool display, int pageIndex, int pageSize)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorksPagingForDisplay]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = display;
            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            
            SqlParameter showNext = cmd.Parameters.Add("@ShowNext", SqlDbType.Bit);
            showNext.Direction = ParameterDirection.Output;

            await cmd.Connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await cmd.Connection.CloseAsync();

            bool output = Implicit.Bool(showNext.Value);
            return output;
        }

        internal static async Task<int> PagingCount(string site, bool display)
        {
            await using var cmd = new SqlCommand("[dbo].[GetWorksPagingForDisplayReturnCount]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@Display", SqlDbType.Bit).Value = display;

            SqlParameter countParameter = cmd.Parameters.Add("@Count", SqlDbType.Int);
            countParameter.Direction = ParameterDirection.Output;

            await cmd.Connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await cmd.Connection.CloseAsync();

            int output = Implicit.Int32(countParameter.Value, 0);
            return output;
        }
    }
}
