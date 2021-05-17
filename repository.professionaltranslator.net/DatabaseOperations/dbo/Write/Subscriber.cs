using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write
{
    internal class Subscriber : Base
    {
        internal static async Task<Result> Delete(string site, Guid id)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[DeleteSubscriber]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                resultStatus = ResultStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Subscriber");
                resultStatus = ResultStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(resultStatus, messages);
        }

        internal static async Task<Result> Delete(string site, string emailAddress)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[DeleteSubscriberForSiteEmailAddress]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 255).Value = site;
                cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 255).Value = emailAddress;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                resultStatus = ResultStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Subscriber");
                resultStatus = ResultStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(resultStatus, messages);
        }

        internal static async Task<Result> Item(string site, Tables.dbo.Subscriber item)
        {
            ResultStatus resultStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[dbo].[SaveSubscriber]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SiteId", SqlDbType.UniqueIdentifier).Value = item.SiteId;
                cmd.Parameters.Add("@AreaId", SqlDbType.Int).Value = item.AreaId;
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = item.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = item.LastName;
                cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 50).Value = item.EmailAddress;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                resultStatus = ResultStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "dbo.Subscriber");
                resultStatus = ResultStatus.Failed;
                messages.Add(ex.Message);
            }

            return new Result(resultStatus, messages);
        }
    }
}
