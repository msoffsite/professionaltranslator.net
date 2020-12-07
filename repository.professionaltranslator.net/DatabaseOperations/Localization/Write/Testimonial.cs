using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Repository.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Write
{
    internal class Testimonial : Base
    {
        internal static async Task<Result> Item(string site, Tables.Localization.Testimonial item)
        {
            SaveStatus saveStatus;
            var messages = new List<string>();

            try
            {
                await using var cmd = new SqlCommand("[Localization].[SaveTestimonial]", new Base().SqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@TestimonialId", SqlDbType.UniqueIdentifier).Value = item.Id;
                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = item.Lcid;
                cmd.Parameters.Add("@Html", SqlDbType.NVarChar, -1).Value = item.Html;
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cmd.Connection.CloseAsync();
                saveStatus = SaveStatus.Succeeded;
            }
            catch (System.Exception ex)
            {
                await Exception.Save(site, ex, "Localization.Testimonial");
                saveStatus = SaveStatus.Failed;
                messages.Add(ex.Message);
            }
            return new Result(saveStatus, messages);
        }
    }
}
