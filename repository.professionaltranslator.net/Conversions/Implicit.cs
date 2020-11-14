using System;
using System.Text.RegularExpressions;

namespace Repository.ProfessionalTranslator.Net.Conversions
{
    public class Implicit
    {
        public enum DefaultYesNo
        {
            Yes,
            No
        }

        /// <summary>
        /// Returns defaultTo if value is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultTo"></param>
        /// <returns></returns>
        public static bool Bool(object value, bool defaultTo)
        {
            bool convertedValue = defaultTo;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    switch (value.ToString().ToLower())
                    {
                        case "0":
                            convertedValue = false;
                            break;
                        case "1":
                            convertedValue = true;
                            break;
                        case "no":
                            convertedValue = false;
                            break;
                        case "yes":
                            convertedValue = true;
                            break;
                        default:
                            bool bValue;
                            if (bool.TryParse(value.ToString(), out bValue))
                            {
                                convertedValue = bValue;
                            }
                            break;
                    }
                }
            }
            catch
            {
                convertedValue = defaultTo;
            }

            return convertedValue;
        }

        /// <summary>
        /// Returns false if value is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Bool(object value)
        {
            return Bool(value, false);
        }

        /// <summary>
        /// Returns no if value is invalid bool.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string BoolYesNo(object value)
        {
            DefaultYesNo convertedValue = DefaultYesNo.No;

            if (Bool(value))
            {
                convertedValue = DefaultYesNo.Yes;
            }

            return convertedValue.ToString();
        }

        public static string BoolYesNo(object value, DefaultYesNo defaultTo)
        {
            DefaultYesNo returnValue = defaultTo;

            bool? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    switch (value.ToString())
                    {
                        case "0":
                            convertedValue = false;
                            break;
                        case "1":
                            convertedValue = true;
                            break;
                        default:
                            bool bValue;
                            if (bool.TryParse(value.ToString(), out bValue))
                            {
                                convertedValue = bValue;
                            }
                            break;
                    }
                }
            }
            catch { }

            if (convertedValue != null)
            {
                switch (convertedValue)
                {
                    case true:
                        returnValue = DefaultYesNo.Yes;
                        break;
                    case false:
                        returnValue = DefaultYesNo.No;
                        break;
                }
            }

            return returnValue.ToString();
        }

        public static string Currency(object value)
        {
            String currency = "$0.00";
            double? convertedValue = Nullable.Double(value);
            if (convertedValue != null)
            {
                currency = convertedValue.Value.ToString("C2");
            }

            return currency;
        }

        public static string Currency(object value, bool format)
        {
            string convertedValue = Currency(value);
            return format ? convertedValue : convertedValue.Replace("$", string.Empty).Replace(",", string.Empty);
        }

        /// <summary>
        /// Returns DateTime.MinValue if object is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime DateTime(object value)
        {
            System.DateTime convertedValue = System.DateTime.MinValue;
            if (value != null)
            {
                System.DateTime testValue;
                if (System.DateTime.TryParse(value.ToString(), out testValue))
                {
                    convertedValue = testValue;
                }
            }


            return convertedValue;
        }

        /// <summary>
        /// Returns defaultTo if value is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultTo"></param>
        /// <returns></returns>
        public static DateTime DateTime(object value, DateTime defaultTo)
        {
            DateTime convertedValue = defaultTo;
            if (Nullable.DateTime(value) != null)
            {
                convertedValue = DateTime(value);
            }

            return convertedValue;
        }

        public static string DateTimeShort(object value)
        {
            string shortenedDate = string.Empty;
            DateTime? convertedValue = Nullable.DateTime(value);
            if (convertedValue != null)
            {
                shortenedDate = convertedValue.Value.ToShortDateString();
            }

            return shortenedDate;
        }

        /// <summary>
        /// Returns -1 if value is invalid decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Decimal(object value)
        {
            decimal safeValue = -1;
            decimal? convertedValue = Nullable.Decimal(value);
            if (convertedValue != null)
            {
                safeValue = (decimal)convertedValue;
            }
            return safeValue;
        }

        public static decimal Decimal(object value, decimal defaultTo)
        {
            decimal convertedValue = Decimal(value);
            if (convertedValue < 0)
            {
                convertedValue = defaultTo;
            }
            return convertedValue;
        }

        /// <summary>
        /// Returns -1 if value is invalid double.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Double(object value)
        {
            double safeValue = -1;
            double? convertedValue = Nullable.Double(value);
            if (convertedValue != null)
            {
                safeValue = (double)convertedValue;
            }
            return safeValue;
        }

        public static double Double(object value, double defaultTo)
        {
            double convertedValue = Double(value);
            if (convertedValue < 0)
            {
                convertedValue = defaultTo;
            }
            return convertedValue;
        }

        /// <summary>
        /// Returns -1 if value is invalid float.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Float(object value)
        {
            float safeValue = -1;
            float? convertedValue = Nullable.Float(value);
            if (convertedValue != null)
            {
                safeValue = (float)convertedValue;
            }
            return safeValue;
        }

        public static float Float(object value, float defaultTo)
        {
            float convertedValue = Float(value);
            if (convertedValue < 0)
            {
                convertedValue = defaultTo;
            }
            return convertedValue;
        }

        /// <summary>
        /// Returns Guid.Empty if value is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static System.Guid Guid(object value)
        {
            Guid convertedValue;
            if (!Guid_TryParse(value.ToString(), out convertedValue))
            {
                convertedValue = System.Guid.Empty;
            };

            return convertedValue;
        }

        public static System.Guid Guid(object value, Guid defaultTo)
        {
            Guid convertedValue = Guid(value);
            if (convertedValue == System.Guid.Empty)
            {
                convertedValue = defaultTo;
            }

            return convertedValue;
        }

        private static bool Guid_TryParse(string s, out Guid result)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            Regex format = new Regex("^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            Match match = format.Match(s);
            if (match.Success)
            {
                result = new Guid(s);
                return true;
            }
            else
            {
                result = System.Guid.Empty;
                return false;
            }
        }

        public static int Int32(object value)
        {
            int convertedValue;
            if (!int.TryParse(value.ToString(), out convertedValue))
            {
                convertedValue = -1;
            };
            return convertedValue;
        }

        public static int Int32(object value, int defaultTo)
        {
            int convertedValue = Int32(value);
            if (convertedValue == -1)
            {
                convertedValue = defaultTo;
            }

            return convertedValue;
        }

        public static long Long(object value)
        {
            long convertedValue;
            if (!long.TryParse(value.ToString(), out convertedValue))
            {
                convertedValue = -1;
            };
            return convertedValue;
        }

        public static long Long(object value, long defaultTo)
        {
            long convertedValue = Long(value);
            if (convertedValue == -1)
            {
                convertedValue = defaultTo;
            }

            return convertedValue;
        }

        public static string String(object value)
        {
            string convertedValue = string.Empty;
            try
            {
                if (value != null)
                {
                    convertedValue = value == DBNull.Value ? string.Empty : value.ToString();
                }
            }
            catch
            {
                convertedValue = string.Empty;
            }

            return convertedValue;
        }

        public static string StringTrim(object value, int characterLimit)
        {
            string input = Implicit.String(value);
            if (input.Length > characterLimit)
            {
                input = input.Substring(0, characterLimit - 3);
                input += "...";
            }
            return input;
        }
    }
}
