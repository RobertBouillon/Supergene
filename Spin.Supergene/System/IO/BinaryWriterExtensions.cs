using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Linq.Expressions;

namespace System.IO
{
  public static class BinaryWriterExtensions
  {
    unsafe public static void WriteStructure(this BinaryWriter _writer, object structure, byte[] buffer, int len)
    {
      fixed (byte* ptr = &buffer[0])
        Marshal.StructureToPtr(structure, (IntPtr)ptr, false);
      _writer.Write(buffer, 0, len);
    }

    unsafe public static T ReadStructure<T>(this BinaryReader reader, byte[] buffer) where T : struct
    {
      T ret;
      reader.Read(buffer, 0, Marshal.SizeOf(typeof(T)));
      fixed (byte* ptr = &buffer[0])
        ret = (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
      return ret;
    }

    /// <summary>
    /// Writes a string with a prefix indicating the length of the string
    /// </summary>
    /// <typeparam name="T">The data type of the length prefix</typeparam>
    /// <param name="writer"></param>
    /// <param name="value">The string to write to the underlying stream</param>
    /// <param name="networkByteOrder">True if the prefix should be formatted in network byte order</param>
    /// <param name="encoding">The encoding of the string</param>
    public static void WriteString<T>(this BinaryWriter writer, string value, bool networkByteOrder, Encoding encoding)
    {
      int len = value.Length;
      if (typeof(T) == typeof(byte))
        writer.Write((byte)len);
      else if (typeof(T) == typeof(short))
        writer.Write((short)len, networkByteOrder);
      else if (typeof(T) == typeof(int))
        writer.Write((int)len, networkByteOrder);
      else
        throw new NotSupportedException(String.Format("String length prefix of type {0} not supported", typeof(T).Name));

      //TODO: Need a bigger buffer if we're dealing with anything but ASCII for both read string and write string.
      byte[] buff = new byte[len];
      encoding.GetBytes(value, 0, len, buff, 0);
      writer.BaseStream.Write(buff, 0, len);
    }

    public static void WriteString(this BinaryWriter writer, string value, int length, Encoding encoding)
    {

      //TODO: Need a bigger buffer if we're dealing with anything but ASCII for both read string and write string.
      byte[] buff = new byte[length];
      encoding.GetBytes(value, 0, value.Length, buff, 0);
      writer.BaseStream.Write(buff, 0, length);
    }

    public static void WriteNullableString(this BinaryWriter writer, string value)
    {
      if (value != null)
      {
        writer.Write((byte)1);
        writer.Write(value);
      }
      else
        writer.Write((byte)0);
    }

    public static void Write(this BinaryWriter writer, short value, bool networkByteOrder)
    {
      if (networkByteOrder)
        writer.Write(IPAddress.HostToNetworkOrder(value));
      else
        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, ushort value, bool networkByteOrder)
    {
      if (networkByteOrder)
        writer.Write(IPAddress.HostToNetworkOrder(value));
      else
        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, int value, bool networkByteOrder)
    {
      if (networkByteOrder)
        writer.Write(IPAddress.HostToNetworkOrder(value));
      else
        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, long value, bool networkByteOrder)
    {
      if (networkByteOrder)
        writer.Write(IPAddress.HostToNetworkOrder(value));
      else
        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, DateTime value) => writer.Write(value.ToBinary());
    public static void Write(this BinaryWriter writer, TimeSpan value) => writer.Write(value.Ticks);
    public static void Write(this BinaryWriter writer, Guid value) => writer.Write(value.ToByteArray());

    public static void Write(this BinaryWriter writer, DateTime? value) { writer.Write(value.HasValue); if (value.HasValue) writer.Write(value.Value.ToBinary()); }
    public static void Write(this BinaryWriter writer, TimeSpan? value) { writer.Write(value.HasValue); if (value.HasValue) writer.Write(value.Value.Ticks); }
    public static void Write(this BinaryWriter writer, Guid? value) { writer.Write(value.HasValue); if (value.HasValue) writer.Write(value.Value.ToByteArray()); }

    public static void Write(this BinaryWriter writer, int? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, short? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, long? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, double? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }
    public static void Write(this BinaryWriter writer, decimal? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, float? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, byte? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, bool? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, uint? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, ushort? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, ulong? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, sbyte? value)
    {
      writer.Write(value.HasValue);
      if (value.HasValue)
        writer.Write(value.Value);
    }

    private static void Iterate(Array data, Action<object> action)
    {
      for (int i = 0; i < data.Length; i++)
        action(data.GetValue(i));
    }

    public static void Write(this BinaryWriter writer, Array data)
    {
      writer.Write(data.Length);

      int count = data.Length;

      var param_data = Expression.Parameter(data.GetType());
      var param_count = Expression.Parameter(typeof(int));
      var param_writer = Expression.Parameter(typeof(BinaryWriter));
      var func = typeof(BinaryWriter).GetMethod("Write", new[] { data.GetType().GetElementType() });

      var loop = ExpressionEx.For(typeof(int), param_count, x => Expression.Call(param_writer, func));
      Expression.Lambda<Func<Array, int, BinaryWriter, Array>>(loop, param_data, param_count, param_writer).Compile()(data, count, writer);
    }
  }
}
