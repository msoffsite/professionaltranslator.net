using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nullable = Repository.ProfessionalTranslator.Net.Conversions.Nullable;

namespace web.professionaltranslator.net.Extensions
{
    internal class Session
    {
        /// <summary>
        /// Manual conversion of GUID necessary due to ChangeType throwing an exception when converting valid GUIDs.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static Guid? Get(ISession session, string key)
        {
            var sessionObject = Get<object>(session, key);
            return Nullable.Guid(sessionObject);
        }

        internal static T Get<T>(ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default : (T)Convert.ChangeType(value, typeof(T));
        }

        internal static void Set<T>(ISession session, string key, object value)
        {
            if (typeof(T) == typeof(int))
            {
                int? test = Nullable.Int32(value);
                if (!test.HasValue)
                {
                    throw new ArgumentException("value must be a valid int.");
                }
                session.SetInt32(key, test.Value);
            }
            else
            {
                if (typeof(T) == typeof(Guid))
                {
                    Guid? test = Nullable.Guid(value);
                    if (!test.HasValue)
                    {
                        throw new ArgumentException("value must be a valid guid.");
                    }
                }
                else if (typeof(T) != typeof(string))
                {
                    throw new ArgumentException("value is an unsupported type.");
                }
                else if (string.IsNullOrEmpty(value.ToString()))
                {
                    throw new ArgumentException("value string cannot be empty");
                }
                session.SetString(key, value.ToString());
            }
        }

        internal static void Set(ISession session, string key, byte[] value)
        {
            session.Set(key, value);
        }

        internal class Json
        {
            internal static T GetObject<T>(ISession session, string key)
            {
                string value = session.GetString(key);
                return value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }

            internal static void SetObject(ISession session, string key, object value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
        }

        internal class Key
        {
            internal const string ClientDataModel = "ClientDataModel";
            internal const string InquiryResult = "InquiryResult";
            internal const string PortfolioDataModel = "PortfoliosDataModel";
            internal const string QueryId = "QueryId";
            internal const string TestimonialDataModel = "TestimonialDataModel";
            internal const string UploadId = "UploadId";
        }
    }
}
