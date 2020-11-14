using System;

namespace Repository.ProfessionalTranslator.Net.Conversions
{
    public class Nullable
    {
        public static bool? Bool(object value)
        {
            Boolean? convertedValue = null;
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
                if ((value == DBNull.Value) &&
                    (value != null))
                {
                    convertedValue = System.DateTime.MinValue;
                }
                else
                {
                    convertedValue = Implicit.DateTime(value);
                }

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
                    decimal tryValue;
                    if (decimal.TryParse(value.ToString(), out tryValue))
                    {
                        convertedValue = (decimal?)tryValue;
                    }
                }
            }
            catch
            {
                convertedValue = null;
            }

            return convertedValue;
        }

        public static Double? Double(object value)
        {
            double? convertedValue = null;
            try
            {
                if ((value != DBNull.Value) &&
                    (value != null))
                {
                    double tryValue;
                    if (double.TryParse(value.ToString(), out tryValue))
                    {
                        convertedValue = (double?)tryValue;
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
                    float tryValue;
                    if (float.TryParse(value.ToString(), out tryValue))
                    {
                        convertedValue = (float?)tryValue;
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
                    int tryValue;
                    if (int.TryParse(value.ToString(), out tryValue))
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
                    long tryValue;
                    if (long.TryParse(value.ToString(), out tryValue))
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
