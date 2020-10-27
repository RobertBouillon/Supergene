using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
  public static class IDataReaderExtension
  {
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
  }
}
