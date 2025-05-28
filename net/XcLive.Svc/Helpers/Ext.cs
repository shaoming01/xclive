using System.Text.RegularExpressions;

namespace SchemaBuilder.Svc.Helpers
{
    public static class Ext
    {
        public static string BytesToHexString(byte[] bytes)
        {
            char[] chArray1 = new char[16]
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F'
            };
            int length = bytes.Length;
            char[] chArray2 = new char[length * 2];
            for (int index = 0; index < length; ++index)
            {
                int num = (int)bytes[index];
                chArray2[index * 2] = chArray1[num >> 4];
                chArray2[index * 2 + 1] = chArray1[num & 15];
            }

            return new string(chArray2);
        }

        public static T Format<T>(T value)
        {
            return value;
        }

        public static Decimal? Format<T>(Decimal? value, int precision)
        {
            return !value.HasValue ? new Decimal?() : new Decimal?(Math.Round(value.Value, precision));
        }

        public static byte[] HexToBytes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return (byte[])null;
            int length = value.Length / 2;
            byte[] bytes = new byte[length];
            for (int index = 0; index < length; ++index)
                bytes[index] = (byte)Convert.ToInt32(value.Substring(index * 2, 2), 16);
            return bytes;
        }

        public static bool ToBoolean(this object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrEmpty(value.ToString()))
                return false;
            int? nullable = value.ToInt();
            int num = 1;
            if (nullable.GetValueOrDefault() == num & nullable.HasValue)
                return true;
            bool result;
            bool.TryParse(value.ToString(), out result);
            return result;
        }


        public static byte[] ToByteArray(object value)
        {
            if (value != null && value != DBNull.Value)
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    try
                    {
                        return (byte[])value;
                    }
                    catch
                    {
                        return (byte[])null;
                    }
                }
            }

            return (byte[])null;
        }

        public static DateTime? ToDateTime(object value)
        {
            DateTime result;
            return value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()) &&
                   DateTime.TryParse(value.ToString(), out result)
                ? new DateTime?(result)
                : new DateTime?();
        }

        public static DateTime? ToDate(this object value)
        {
            return ToDateTime(value);
        }

        public static string ToDbString(string value)
        {
            return Regex.Replace(value, "\\W", "_");
        }

        public static object ToDbValue(bool value)
        {
            return (object)(value ? 1 : 0);
        }

        public static object ToDbValue(DateTime? value)
        {
            return !value.HasValue ? (object)DBNull.Value : (object)value.Value;
        }

        public static object ToDbValue(Decimal? value)
        {
            return !value.HasValue ? (object)DBNull.Value : (object)value.Value;
        }

        public static object ToDbValue(object value)
        {
            return value == null ? (object)DBNull.Value : value;
        }

        public static object ToDbValue(int? value)
        {
            return !value.HasValue ? (object)DBNull.Value : (object)value.Value;
        }

        public static object ToDbValue(string value)
        {
            return string.IsNullOrEmpty(value) ? (object)DBNull.Value : (object)value;
        }

        public static Decimal? ToDecimal(this object value)
        {
            Decimal result;
            return value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()) &&
                   Decimal.TryParse(value.ToString(), out result)
                ? new Decimal?(result)
                : new Decimal?();
        }

        public static double? ToDouble(object value)
        {
            double result;
            return value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()) &&
                   double.TryParse(value.ToString(), out result)
                ? new double?(result)
                : new double?();
        }

        public static int? ToInt(this object value)
        {
            if (value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()))
            {
                string s = value.ToString();
                int result1;
                if (int.TryParse(s, out result1))
                    return new int?(result1);
                Decimal result2;
                if (Decimal.TryParse(s, out result2) && result2 > -2147483648M && result2 < 2147483647M)
                    return new int?(Convert.ToInt32(result2));
            }

            return new int?();
        }

        public static long? ToLong(this object? value)
        {
            if (value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()))
            {
                string s = value.ToString();
                long result1;
                if (long.TryParse(s, out result1))
                    return new long?(result1);
                Decimal result2;
                if (Decimal.TryParse(s, out result2) && result2 > -9223372036854775808M &&
                    result2 < 9223372036854775807M)
                    return new long?(Convert.ToInt64(result2));
            }

            return new long?();
        }

        public static object ToObject(object value)
        {
            return value;
        }

        public static string ToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

    }
}