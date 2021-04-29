using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public static class BinaryReaderExtensions
  {
    /// <summary>
    /// Reads a fixed-length null terminated string
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static string ReadFixedLengthString(this BinaryReader reader, int len)
    {
      return new String(reader.ReadChars(len)).Trim('\0');
    }

    public static void Read(this BinaryReader reader, ref short target) => target = reader.ReadInt16();
    public static void Read(this BinaryReader reader, ref int target) => target = reader.ReadInt32();
    public static void Read(this BinaryReader reader, ref long target) => target = reader.ReadInt64();
    public static void Read(this BinaryReader reader, ref ushort target) => target = reader.ReadUInt16();
    public static void Read(this BinaryReader reader, ref uint target) => target = reader.ReadUInt32();
    public static void Read(this BinaryReader reader, ref ulong target) => target = reader.ReadUInt64();
    public static void Read(this BinaryReader reader, ref float target) => target = reader.ReadSingle();
    public static void Read(this BinaryReader reader, ref double target) => target = reader.ReadDouble();
    public static void Read(this BinaryReader reader, ref decimal target) => target = reader.ReadDecimal();

    public static void Read(this BinaryReader reader, ref short? target) => target = reader.ReadNullableInt16();
    public static void Read(this BinaryReader reader, ref int? target) => target = reader.ReadNullableInt32();
    public static void Read(this BinaryReader reader, ref long? target) => target = reader.ReadNullableInt64();
    public static void Read(this BinaryReader reader, ref ushort? target) => target = reader.ReadNullableUInt16();
    public static void Read(this BinaryReader reader, ref uint? target) => target = reader.ReadNullableUInt32();
    public static void Read(this BinaryReader reader, ref ulong? target) => target = reader.ReadNullableUInt64();
    public static void Read(this BinaryReader reader, ref float? target) => target = reader.ReadNullableSingle();
    public static void Read(this BinaryReader reader, ref double? target) => target = reader.ReadNullableDouble();
    public static void Read(this BinaryReader reader, ref decimal? target) => target = reader.ReadNullableDecimal();


    public static void Read(this BinaryReader reader, ref string target) => target = reader.ReadString();
    public static void ReadNullable(this BinaryReader reader, ref string target) => target = reader.ReadNullableString();

    public static DateTime ReadDateTime(this BinaryReader reader) => DateTime.FromBinary(reader.ReadInt64());
    public static TimeSpan ReadTimeSpan(this BinaryReader reader) => TimeSpan.FromTicks(reader.ReadInt64());
    public static Guid ReadGuid(this BinaryReader reader) => new Guid(reader.ReadBytes(16));

    public static DateTime? ReadNullableDateTime(this BinaryReader reader) => reader.ReadBoolean() ? (DateTime?)DateTime.FromBinary(reader.ReadInt64()) : null;
    public static TimeSpan? ReadNullableTimeSpan(this BinaryReader reader) => reader.ReadBoolean() ? (TimeSpan?)TimeSpan.FromTicks(reader.ReadInt64()) : null;
    public static Guid? ReadNullableGuid(this BinaryReader reader) => reader.ReadBoolean() ? (Guid?)new Guid(reader.ReadBytes(16)) : null;


    public static void Read<T>(this BinaryReader reader, List<T> list, Func<BinaryReader, T> factory)
    {
      int count = reader.ReadInt32();
      for (int i = 0; i < count; i++)
        list.Add(factory(reader));
    }

    public static void Read<T>(this BinaryReader reader, ref List<T> list, Func<BinaryReader, T> factory)
    {
      int count = reader.ReadInt32();
      list = new List<T>(count);
      for (int i = 0; i < count; i++)
        list.Add(factory(reader));
    }


    public static string ReadNullableString(this BinaryReader reader)
    {
      int hasValue = reader.ReadByte();
      if (hasValue == 1)
        return reader.ReadString();
      return null;
    }

    /// <summary>
    /// Reads a string from the underlying stream, prefixed by string length
    /// </summary>
    /// <typeparam name="T">The type of data specifying the length of the stream</typeparam>
    /// <param name="reader"></param>
    /// <param name="networkByteOrder">True if the data prefix is network byte order.</param>
    /// <param name="encoding">The type of encoding</param>
    /// <returns></returns>
    public static string ReadString<T>(this BinaryReader reader, bool networkByteOrder, Encoding encoding)
    {
      int len;
      if (typeof(T) == typeof(byte))
        len = reader.ReadByte();
      else if (typeof(T) == typeof(short))
        len = reader.ReadInt16(networkByteOrder);
      else if (typeof(T) == typeof(int))
        len = reader.ReadInt32(networkByteOrder);
      else
        throw new NotSupportedException(String.Format("String length prefix of type {0} not supported", typeof(T).Name));

      byte[] buff = new byte[len];
      reader.BaseStream.Read(buff, 0, len);
      return encoding.GetString(buff, 0, len);
    }

    public static string ReadString(this BinaryReader reader, int length, byte[] buffer)
    {
      Stream s = reader.BaseStream;
      s.Read(buffer, 0, length);
      return ASCIIEncoding.ASCII.GetString(buffer, 0, length);
    }


    public static short ReadInt16(this BinaryReader reader, bool networkByteOrder)
    {
      if (networkByteOrder)
        return (short)((reader.ReadByte() << 8) + reader.ReadByte()); // return IPAddress.NetworkToHostOrder(reader.ReadInt16());
      else
        return reader.ReadInt16();
    }

    public static ushort ReadUInt16(this BinaryReader reader, bool networkByteOrder)
    {
      if (networkByteOrder)
        return (ushort)((reader.ReadByte() << 8) + reader.ReadByte()); // return IPAddress.NetworkToHostOrder(reader.ReadInt16());
      else
        return reader.ReadUInt16();
    }

    public static int ReadInt32(this BinaryReader reader, bool networkOrder)
    {
      if (networkOrder)
        return IPAddress.NetworkToHostOrder(reader.ReadInt32());
      else
        return reader.ReadInt32();
    }

    public static long Read(this BinaryReader reader, bool networkOrder)
    {
      if (networkOrder)
        return IPAddress.NetworkToHostOrder(reader.ReadInt64());
      else
        return reader.ReadInt64();
    }

    public static long? ReadNullableInt64(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadInt64();
    }

    public static int? ReadNullableInt32(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadInt32();
    }

    public static short? ReadNullableInt16(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadInt16();
    }

    public static ulong? ReadNullableUInt64(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadUInt64();
    }

    public static uint? ReadNullableUInt32(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadUInt32();
    }

    public static ushort? ReadNullableUInt16(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadUInt16();
    }

    public static Single? ReadNullableSingle(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadSingle();
    }

    public static Double? ReadNullableDouble(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadDouble();
    }

    public static Decimal? ReadNullableDecimal(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadDecimal();
    }

    public static byte? ReadNullableByte(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadByte();
    }

    public static sbyte? ReadNullableSByte(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadSByte();
    }

    public static bool? ReadNullableBoolean(this BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return null;
      return reader.ReadBoolean();
    }

    private static Dictionary<Type, MethodInfo> TypeMethods = new Dictionary<Type, MethodInfo>
    {
      { typeof(short), typeof(BinaryReader).GetMethod("ReadInt16") },
      { typeof(int), typeof(BinaryReader).GetMethod("ReadInt32") },
      { typeof(long), typeof(BinaryReader).GetMethod("ReadInt64") },

      { typeof(ushort), typeof(BinaryReader).GetMethod("ReadUInt16") },
      { typeof(uint), typeof(BinaryReader).GetMethod("ReadUInt32") },
      { typeof(ulong), typeof(BinaryReader).GetMethod("ReadUInt64") },

      { typeof(byte), typeof(BinaryReader).GetMethod("ReadByte") },
      { typeof(sbyte), typeof(BinaryReader).GetMethod("ReadSByte") },

      { typeof(float), typeof(BinaryReader).GetMethod("ReadFloat") },
      { typeof(double), typeof(BinaryReader).GetMethod("ReadDouble") },
      { typeof(decimal), typeof(BinaryReader).GetMethod("ReadDecimal") },

      { typeof(string), typeof(BinaryReader).GetMethod("ReadString") }
    };

    public static Array ReadArray(this BinaryReader reader, Type type)
    {
      int count = reader.ReadInt32();
      var ret = Array.CreateInstance(type, count);

      var param_data = Expression.Parameter(type.MakeArrayType());
      var param_count = Expression.Parameter(typeof(int));
      var param_reader = Expression.Parameter(typeof(BinaryReader));

      if (!TypeMethods.TryGetValue(type, out var func))
        throw new Exception($"Unable to deserialize {type.Name}");

      var loop = ExpressionEx.For(typeof(int), param_count, x => Expression.Assign(Expression.ArrayIndex(param_data, x), Expression.Call(param_reader, func)));
      return Expression.Lambda<Func<Array, int, BinaryReader, Array>>(loop, param_data, param_count, param_reader).Compile()(ret, count, reader);
    }

    public static T[] ReadArray<T>(this BinaryReader reader) => (T[])ReadArray(reader, typeof(T));
  }
}
