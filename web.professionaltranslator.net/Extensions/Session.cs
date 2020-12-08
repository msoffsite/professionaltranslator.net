using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
            return Repository.ProfessionalTranslator.Net.Conversions.Nullable.Guid(sessionObject);
        }

        internal static T Get<T>(ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default : (T)Convert.ChangeType(value, typeof(T));
        }

        internal static void Set(ISession session, string key, byte[] value)
        {
            session.Set(key, value);
        }

        internal static void Set(ISession session, string key, int value)
        {
            session.SetInt32(key, value);
        }

        internal static void Set(ISession session, string key, string value)
        {
            session.SetString(key, value);
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
            internal const string InquiryResult = "InquiryResult";
        }
    }
}
