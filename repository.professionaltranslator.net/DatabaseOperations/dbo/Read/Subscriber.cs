using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net.Conversions;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read
{
    internal class Subscriber : Base
    {
        internal static async Task<Tables.dbo.Subscriber> Item(Guid id)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSubscriber]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Subscriber>(sda);
        }

        internal static async Task<Tables.dbo.Subscriber> Item(string site, int areaId, string emailAddress)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSubscriberForSiteAreaIdEmailAddress]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@AreaId", SqlDbType.Int, 25).Value = areaId;
            cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 255).Value = emailAddress;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.dbo.Subscriber>(sda);
        }

        internal static async Task<List<Tables.dbo.Subscriber>> List(string site, int areaId)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSubscribersForSiteAreaId]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@AreaId", SqlDbType.Int).Value = areaId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Subscriber>(sda);
        }

        internal static async Task<List<Tables.dbo.Subscriber>> List(string site, int areaId, int pageIndex, int pageSize)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSubscribersPaging]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@AreaId", SqlDbType.Int).Value = areaId;
            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.dbo.Subscriber>(sda);
        }

        internal static async Task<int> PagingCount(string site, int areaId)
        {
            await using var cmd = new SqlCommand("[dbo].[GetSubscribersPagingReturnCount]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 25).Value = site;
            cmd.Parameters.Add("@AreaId", SqlDbType.Int).Value = areaId;

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