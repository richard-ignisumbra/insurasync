using System.Data.SqlClient;
using System;
namespace utilities
{
    public static class SQLFunctions
    {

        public static object DBNullIfEmptyString(this string Text, int length)
        {
            return (String.IsNullOrEmpty(Text)) ? (object)DBNull.Value : Text.Left(length);
        }
        public static object EmptyIfNullString(this string Text, int length)
        {
            return (String.IsNullOrEmpty(Text)) ? (object)"" : Text.Left(length);
        }
        public static string EmptyIfDBNullString(this Object Text, int? length = null)
        {
            if (Text == System.DBNull.Value || Text == null)
            {
                return String.Empty;
            }
            else
            {
                var result = Text.ToString();
                if (length.HasValue)
                {
                    result = result.Left(length.Value);
                }
                return result;
            }
        }
        public static DateTime? EmptyIfDBNullDate(this Object theValue)
        {
            if (theValue == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (System.DateTime)theValue;
            }
        }
        public static int? NullifDBNullInt(this Object theValue)
        {
            if (theValue == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (int)theValue;
            }
        }
        public static object DBNullIfNullString(this string Text, int length)
        {
            return (Text == null) ? (object)DBNull.Value : Text.Left(length);
        }

        public static object DBNullIfNullInt(this int? number)
        {
            return (number.HasValue) ? (object)number.Value : (object)DBNull.Value;
        }
        public static object DBNullIfNullDecimal(this decimal? number)
        {
            return (number.HasValue) ? (object)number.Value : (object)DBNull.Value;
        }
        public static int asInt(this object dataReaderObject)
        {
            return (int)dataReaderObject;
        }
        public static decimal asDecimal(this object dataReaderObject)
        {
            return (decimal)dataReaderObject;
        }

        public static int? asNullableInt(this object dataReaderObject)
        {
            if (dataReaderObject == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (int)dataReaderObject;
            }

        }
        public static decimal? asNullableDecimal(this object dataReaderObject)
        {
            if (dataReaderObject == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (decimal)dataReaderObject;
            }

        }
        public static Boolean? asNullableBool(this object dataReaderObject)
        {
            if (dataReaderObject == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (Boolean)dataReaderObject;
            }

        }
        public static string asString(this object dataReaderObject)
        {
            if (DBNull.Value.Equals(dataReaderObject))
            {
                return string.Empty;
            }
            return (string)dataReaderObject;
        }

        public static DateTime asDate(this object dataReaderObject)
        {
            return (DateTime)dataReaderObject;
        }
        public static DateTime? asNullableDate(this object dataReaderObject)
        {
            if (dataReaderObject == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return (DateTime)dataReaderObject;
            }

        }

        public static Boolean asBoolean(this object dataReaderObject)
        {
            if (dataReaderObject == System.DBNull.Value)
            {
                return false;
            }
            else
            {
                return (Boolean)dataReaderObject;
            }

        }
        public static object DBNullIfNullDate(this DateTime? theDate)
        {
            return (theDate.HasValue) ? (object)theDate.Value : (object)DBNull.Value;
        }

        public static object DBNullIfNullBoolean(this Boolean? theBool)
        {
            return (theBool.HasValue) ? (object)theBool.Value : (object)DBNull.Value;
        }

    }


}