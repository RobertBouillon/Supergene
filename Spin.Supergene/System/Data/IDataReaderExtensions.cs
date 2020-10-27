using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace System.Data
{
  public static class IDataReaderExtension
  {
    public static IEnumerable<IEnumerable<object>> Enumerate(this IDataReader reader)
    {
      var cols = reader.FieldCount;
      object[] data = new object[cols];

      while (reader.Read())
      {
        reader.GetValues(data); //Faster than using the indexer on reader
        yield return data;
      }
    }

    //public static IEnumerable<IDataReader> Enumerate(this IDataReader reader)
    //{
    //  while (reader.Read())
    //    yield return reader;
    //}

    public static T GetEnum<T>(this IDataReader reader, int ordinal) where T: struct
    {
      return (T)Enum.ToObject(typeof(T), reader.GetValue(ordinal));
    }

    public static T GetEnum<T>(this IDataReader reader, string fieldName) where T: struct
    {
      return GetEnum<T>(reader, reader.GetOrdinal(fieldName));
    }

    public static int GetInt32(this IDataReader reader, string ordinal)
    {
      return Convert.ToInt32(reader.GetValue(reader.GetOrdinal(ordinal)));
    }

    public static string GetString(this IDataReader reader, string ordinal)
    {
      return reader.GetString(reader.GetOrdinal(ordinal));
    }

    public static string GetNullableString(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }


    public static short? GetNullableInt16(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (short?) null : reader.GetInt16(ordinal);
    }

    public static int? GetNullableInt32(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (int?) null : reader.GetInt32(ordinal);
    }

    public static long? GetNullableInt64(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (long?) null : reader.GetInt64(ordinal);
    }

    public static double? GetNullableDouble(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (double?)null : reader.GetDouble(ordinal);
    }

    public static DateTime? GetNullableDateTime(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
    }

    public static TimeSpan? GetNullableTimeSpan(this IDataReader reader, int ordinal)
    {
      throw new NotImplementedException();
      //var sreader = reader as SqlDataReader;
      //if(sreader!=null)
      //  return sreader.IsDBNull(ordinal) ? (TimeSpan?)null : sreader.GetTimeSpan(ordinal);
      //else
      //  return reader.IsDBNull(ordinal) ? (TimeSpan?)null : reader.GetDateTime(ordinal).TimeOfDay;
    }

    public static decimal? GetNullableDecimal(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (decimal?) null : reader.GetDecimal(ordinal);
    }

    public static float? GetNullableSingle(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);
    }

    public static byte? GetNullableByte(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (byte?)null : reader.GetByte(ordinal);
    }

    public static bool? GetNullableBoolean(this IDataReader reader, int ordinal)
    {
      return reader.IsDBNull(ordinal) ? (bool?) null : reader.GetBoolean(ordinal);
    }


  }
}
