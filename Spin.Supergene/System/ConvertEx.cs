using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class ConvertEx
  {
    public static T ChangeType<T>(object value) where T : struct => (T)ChangeType(value, typeof(T));

    public static object ChangeType(object value, Type conversionType)
    {
      if (value.GetType() == conversionType)
        return value;

      if (value is Int128 i128)
        if (conversionType == typeof(sbyte))
          return (sbyte)i128;
        else if (conversionType == typeof(short))
          return (short)i128;
        else if (conversionType == typeof(int))
          return (int)i128;
        else if (conversionType == typeof(long))
          return (long)i128;
        else if (conversionType == typeof(byte))
          return (byte)i128;
        else if (conversionType == typeof(ushort))
          return (ushort)i128;
        else if (conversionType == typeof(uint))
          return (uint)i128;
        else if (conversionType == typeof(ulong))
          return (ulong)i128;
        else
          throw new Exception($"Cannot convert Int128 to {conversionType.Name}");
      else if (conversionType == typeof(Int128))
        if (value is sbyte sb)
          return (Int128)sb;
        else if (value is short s)
          return (Int128)s;
        else if (value is int i)
          return (Int128)i;
        else if (value is long l)
          return (Int128)l;
        else if (value is byte b)
          return (Int128)b;
        else if (value is ushort us)
          return (Int128)us;
        else if (value is uint ui)
          return (Int128)ui;
        else if (value is ulong ul)
          return (Int128)ul;
        else
          throw new Exception($"Cannot convert '{value}' to Int128");
      else
        return Convert.ChangeType(value, conversionType);
    }
  }
}
