using System;

namespace Repository.ProfessionalTranslator.Net.Conversions
{
    public class Nullable
    {
        public static bool? Bool(object value)
        {
            bool? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    convertedValue = Implicit.Bool(value);
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static DateTime? DateTime(object value)
        {
            DateTime? convertedValue;
            try
            {
                convertedValue = value == DBNull.Value ? System.DateTime.MinValue : Implicit.DateTime(value);

                if (convertedValue == System.DateTime.MinValue)
                {
                    convertedValue = null;
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static decimal? Decimal(object value)
        {
            decimal? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    if (decimal.TryParse(value.ToString(), out decimal tryValue))
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static double? Double(object value)
        {
            double? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    if (double.TryParse(value.ToString(), out double tryValue))
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static float? Float(object value)
        {
            float? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    if (float.TryParse(value.ToString(), out float tryValue))
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static Guid? Guid(object value)
        {
            Guid? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    Guid tryValue = Implicit.Guid(value);
                    if (tryValue != System.Guid.Empty)
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static int? Int32(object value)
        {
            int? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    if (int.TryParse(value.ToString(), out int tryValue))
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static long? Long(object value)
        {
            long? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    if (long.TryParse(value.ToString(), out long tryValue))
                    {
                        convertedValue = tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }
    }
}
