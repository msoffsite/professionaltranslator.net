using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Object = Repository.ProfessionalTranslator.Net.Conversions.Object;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Upload.Read
{
    internal class Client : Base
    {
        internal static async Task<Tables.Upload.Client> Item(Guid? id)
        {
            await using var cmd = new SqlCommand("[Upload].[GetClient]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.Upload.Client>(sda);
        }

        internal static async Task<Tables.Upload.Client> Item(Guid? clientId, string originalFilename)
        {
            await using var cmd = new SqlCommand("[Upload].[GetClientByClientIdOriginalFilename]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@ClientId", SqlDbType.UniqueIdentifier).Value = clientId;
            cmd.Parameters.Add("@OriginalFilename", SqlDbType.NVarChar, 256).Value = originalFilename;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetItem<Tables.Upload.Client>(sda);
        }

        internal static async Task<List<Tables.Upload.Client>> List(Guid? clientId)
        {
            await using var cmd = new SqlCommand("[Upload].[GetClients]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@ClientId", SqlDbType.UniqueIdentifier).Value = clientId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Upload.Client>(sda);
        }

        internal static async Task<List<Tables.Upload.Client>> List(Guid? clientId, Guid? inquiryId)
        {
            await using var cmd = new SqlCommand("[Upload].[GetClientsLimitByInquiry]", new Base().SqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@ClientId", SqlDbType.UniqueIdentifier).Value = clientId;
            cmd.Parameters.Add("@InquiryId", SqlDbType.UniqueIdentifier).Value = inquiryId;
            using var sda = new SqlDataAdapter(cmd);
            return Object.GetList<Tables.Upload.Client>(sda);
        }
    }
}