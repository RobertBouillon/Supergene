using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace System.Runtime.InteropServices
{
  public enum EndianOrder
  {
    BigEndian,
    LittleEndian
  }
  /// <summary>
  /// Tools extending the InteropServices Namespace designed specifically for CISPoll
  /// </summary>
  public class MarshalEx
  {
    #region Mask Methods
    /// <summary>
    /// Evaluates a character mask.
    /// </summary>
    /// <param name="bp">The starting byte where the mask exists</param>
    /// <param name="Mask">The mask to compare</param>
    /// <param name="Length">Size of the data to evaluate</param>
    /// <returns></returns>
    unsafe public static bool EvalMask(int* bp, uint Mask, ushort Length)
    {
      int offset = 1;
      do
      {
        offset++;
        Mask >>= 8;
      } while (Mask > 255);
      Mask = Mask;
      bp += (Length - offset);
      return ((*bp & Mask) == Mask);
    }

    unsafe public static void WriteMask(int* Destination, uint Mask, ushort Length)
    {
      int offset = 0;
      do
      {
        offset++;
        Mask >>= 8;
      } while (Mask > 255);
      Mask = Mask;
      Destination += (Length - offset);
      *Destination += (byte)Mask;
    }
    #endregion
    #region Misc. Conversions
    unsafe public static void WriteString(byte* Destination, string Source)
    {
      IntPtr src = Marshal.StringToHGlobalAnsi(Source);
      int len = Source.Length;
      byte* psrc = (byte*)src;
      for (int i = 0; i < len; i++)
      {
        char watch = (char)*psrc;
        *Destination = *psrc;
        psrc++;
        Destination++;
      }
      Marshal.FreeHGlobal(src);
    }

    unsafe public static byte[] ReadBytes(IntPtr Source, uint Length)
    {
      byte[] ret = new byte[Length];
      byte* bp = (byte*)Source;
      for (int i = 0; i < Length; i++)
      {
        ret[i] = *bp;
        bp++;
      }
      return ret;
    }

    unsafe public static uint SwitchEndian(uint ToConvert)
    {
      uint ret = 0;
      byte* retp = (byte*)&ret;
      byte* toconv = (byte*)&ToConvert;
      retp[0] = toconv[3];
      retp[1] = toconv[2];
      retp[2] = toconv[1];
      retp[3] = toconv[0];
      return ret;
    }

    unsafe public static ushort SwitchEndian(ushort toConvert)
    {
      ushort ret = 0;
      byte* retp = (byte*)&ret;
      byte* toconv = (byte*)&toConvert;
      retp[0] = toconv[1];
      retp[1] = toconv[0];
      return ret;
    }
    #endregion
    #region Struct Parsing

    public static byte[] StructureArrayToBytes(object[] source)
    {
      #region Validation
      if (source == null)
        throw new ArgumentNullException("source");

      if (source.Length == 0)
        throw new ArgumentException("Source array cannot be empty.", "source");
      #endregion
      int size = Marshal.SizeOf(source[0]);
      MemoryStream ms = new MemoryStream(size * source.Length);
      foreach (object o in source)
        ms.Write(StructureToBytes(o), 0, size);
      return ms.ToArray();
    }

    /// <summary>
    /// Converts an array of bytes to an array of structures.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="arraySize"></param>
    /// <returns></returns>
    public static object[] BytesToStructureArray(byte[] source, Type destination, int arraySize)
    {
      #region Validation
      if (source.Length != (Marshal.SizeOf(destination) * arraySize))
        throw new InvalidCastException("Source length does not match the size of the Destination");

      if (!destination.IsLayoutSequential)
        throw new InvalidCastException("Destination must be a struct with the Layout Sequential attribute set");

      #endregion

      //TODO:If the destination is blittable, use pointers and populate the struct directly with a pinned GCHandle

      int size = Marshal.SizeOf(destination);
      IntPtr conv = Marshal.AllocHGlobal(size * arraySize);
      object[] ret = new object[arraySize];
      try
      {
        unsafe
        {
          byte* bp = (byte*)conv;
          foreach (byte s in source)
          {
            *bp = s;
            bp++;
          }
          bp = (byte*)conv;           //Clone our Pointer
          for (int i = 0; i < arraySize; i++)
          {
            ret[i] = Marshal.PtrToStructure((IntPtr)bp, destination);
            bp += size;
          }
        }
      }
      finally
      {
        Marshal.FreeHGlobal(conv);
      }
      return ret;
    }

    /// <summary>
    /// Converts an array of bytes to an array of structures.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    /// <remarks>Use blittable types when possible. If the destination is not blittable, then the data must be copied to unmanaged memory and then re-created in the managed heap</remarks>
    public static object BytesToStructure(byte[] source, Type destination)
    {
      #region Validation
      if (source.Length != Marshal.SizeOf(destination))
        throw new InvalidCastException("Source length does not match the size of the Destination");

      if (!destination.IsLayoutSequential)
        throw new InvalidCastException("Destination must be a struct with the Layout Sequential attribute set");

      #endregion

      IntPtr conv = Marshal.AllocHGlobal(Marshal.SizeOf(destination));
      object ret;
      try
      {
        unsafe
        {
          byte* bp = (byte*)conv;
          foreach (byte s in source)
          {
            *bp = s;
            bp++;
          }
        }
        ret = Marshal.PtrToStructure(conv, destination);
      }
      finally
      {
        Marshal.FreeHGlobal(conv);
      }
      return ret;
    }

    public static byte[] StructureToBytes(object source)
    {
      #region Validation
      if (source == null)
        throw new ArgumentNullException("source");
      if (!source.GetType().IsLayoutSequential)
        throw new InvalidCastException("Source object must be a struct with the Layout Sequential attribute set");
      #endregion

      int size = Marshal.SizeOf(source);
      IntPtr conv = Marshal.AllocHGlobal(size);
      byte[] ret = new byte[size];
      try
      {
        Marshal.StructureToPtr(source, conv, false);
        unsafe
        {
          byte* bp = (byte*)conv;
          for (int i = 0; i < size; i++)
          {
            ret[i] = *bp;
            bp++;
          }
        }
      }
      finally
      {
        Marshal.FreeHGlobal(conv);
      }
      return ret;
    }
    #endregion
    #region BinarySum
    /// <summary>
    /// Sums the contents of the structure
    /// </summary>
    /// <param name="source">The object to sum</param>
    /// <param name="escapeChar">These characters are doubled when encountered. Ignored if value is 0x00.</param>
    /// <returns>The binary sum</returns>
    public static byte BinarySum(object source, byte escapeChar)
    {
      #region Validation
      if (source == null)
        throw new ArgumentNullException("source");
      if (!source.GetType().IsLayoutSequential)
        throw new InvalidCastException("Source object must be a struct with the Layout Sequential attribute set");
      #endregion

      int size = Marshal.SizeOf(source);
      IntPtr conv = Marshal.AllocHGlobal(size);

      byte ret = 0;
      try
      {
        Marshal.StructureToPtr(source, conv, false);
        unsafe
        {
          byte* bp = (byte*)conv;
          for (int i = 0; i < size; i++)
          {
            ret += *bp;
            if (*bp == escapeChar)
              if (escapeChar != 0x00)
                ret += *bp;

            bp++;
          }
        }
      }
      finally
      {
        Marshal.FreeHGlobal(conv);
      }
      return ret;
    }

    public static byte BinarySum(byte[] source)
    {
      byte ret = 0x00;
      foreach (byte b in source)
        ret += b;

      return ret;
    }

    public static byte BinarySum(object source)
    {
      return BinarySum(source, 0x00);
    }
    #endregion

    public static void DumpBytes(byte[] toDump)
    {
      foreach (byte b in toDump)
        Console.Out.Write(b.ToString("X2") + " ");
    }

    unsafe public static bool ArrayCompare(byte[] a, byte[] b)
    {
      #region Validation
      if (a == null)
        throw new ArgumentNullException("a");
      if (b == null)
        throw new ArgumentNullException("b");
      #endregion
      if (a.Length != b.Length)
        return false;

      for (int i = 0; i < a.Length; i++)
        if (a[i] != b[i])
          return false;

      return true;
    }

    unsafe public static bool ArrayCompare(char[] a, char[] b)
    {
      #region Validation
      if (a == null)
        throw new ArgumentNullException("a");
      if (b == null)
        throw new ArgumentNullException("b");
      #endregion
      if (a.Length != b.Length)
        return false;

      for (int i = 0; i < a.Length; i++)
        if (a[i] != b[i])
          return false;

      return true;
    }

    public static object InvokeGeneric<T>(string name, Type[] typeArgs, params object[] args) => InvokeGeneric(typeof(T), name, typeArgs, args);

    public static object InvokeGeneric(Type type, string name, Type[] typeArgs, params object[] args)
    {
      var parameters = args.Select(x => Expression.Parameter(x.GetType())).ToArray();

      Expression body = null;
      try
      {
        body = Expression.Call(type, name, typeArgs, parameters);
      }
      catch(Exception ex)
      {
        var method = type.GetMethods(Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance)
          .Where(x => x.Name == name && x.IsGenericMethod)
          .Where(x => x.GetGenericArguments().Length == typeArgs.Length)
          .Select(x => x.MakeGenericMethod(typeArgs))
          .Where(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(args.Select(y => y.GetType())))
          .SingleOrDefault();

        if (method != null)
          throw new Exception($"'{name}<{String.Join(",", typeArgs.Select(x=>x.Name))}>' must be a static method");
        else
          throw;
      }
      var allparameters = parameters.ToArray();

      var op = Expression.Lambda(body, allparameters).Compile();
      var ret = op.DynamicInvoke(args);
      return ret;
    }

    public static object InvokeGeneric(object target, string name, Type[] typeArgs, params object[] args)
    {
      var instance = Expression.Parameter(target.GetType());
      var parameters = args.Select(x => Expression.Parameter(x.GetType())).ToArray();
      var body = Expression.Call(instance, name, typeArgs, parameters);
      var allparameters = parameters.Concat(new[] { instance }).ToArray();

      var op = Expression.Lambda(body, allparameters).Compile();
      var ret = op.DynamicInvoke(args.Concat(new[] { target }).ToArray());
      return ret;
    }

    #region BCD/INT conversion
    public static byte[] Int32ToBcd(uint number, int length)
    {
      #region Validation
      if (length < 1 || length > 15)
        throw new ArgumentOutOfRangeException("length");
      #endregion
      ulong bcd = 0;
      byte[] tmp = new byte[8];
      byte[] ret = new byte[length];

      while (number > 0)
      {
        bcd |= ((number / 10) % 10) << 4;
        bcd += number % 10;

        number /= 100;
        bcd <<= 8;
      }
      bcd >>= 8;

      tmp = BitConverter.GetBytes(bcd);

      int position = 7;
      while (tmp[position] == 0 && position > 0)
        position--;

      if (position >= length)
        throw new ArgumentOutOfRangeException("length", "length must be greater than or equal to the length of the actual number.");

      int retposition = length - 1;
      for (; position >= 0; position--)
        ret[retposition--] = tmp[position];

      return ret;
    }

    public static uint BcdToInt32(byte bcd)
    {
      return BcdToInt32(new byte[] { bcd });
    }

    public static uint BcdToInt32(byte[] bcd)
    {
      return BcdToInt32(bcd, 0, bcd.Length);
    }

    /// <summary>
    /// Converts a Binary Coded Decimal into an Int32
    /// </summary>
    /// <param name="bcd">The BCD to convert</param>
    /// <param name="start">The index of the array in which to start</param>
    /// <param name="length">The index in the array in which to end</param>
    /// <returns></returns>
    public static uint BcdToInt32(byte[] bcd, int start, int length)
    {
      #region Validation
      if (bcd == null)
        throw new ArgumentNullException("bcd");
      #endregion
      uint ret = 0;
      for (int i = (start + length - 1); i >= start; i--)
        ret += (uint)((((bcd[i] >> 4) * 10) + bcd[i] % 16) * Math.Pow(10, 2 * (((start + length - 1) - i))));
      //ret+=(uint)((((bcd[i]>>4)*10)+bcd[i]%16)*Math.Pow(10,2*((bcd.Length-i)-1)));
      return ret;
    }

    #endregion
    #region BCD/Decimal conversion
    public static byte[] DecimalToBcd(decimal bcd, int returnSize)
    {
      return DecimalToBcd(bcd, returnSize, false);
    }

    public static byte[] DecimalToBcd(decimal bcd, int returnSize, bool signed)
    {
      #region Validation
      decimal max = (decimal)Math.Pow(10, returnSize * 2) - 1;
      if (signed)
        max /= 100;
      if (bcd > max)
        throw new ArgumentException(String.Format("Decimal value 'bcd' is greater than the return size specified (bcd value: {0} max value: {1})", bcd, max));
      #endregion
      byte[] ret = new byte[returnSize];

      decimal working = bcd;
      int index = returnSize - 1;

      if (signed)
      {
        ret[index] = 0xF0;
        if (bcd < 0)
          ret[index] |= 0x01;
        index--;
      }

      while (Decimal.Truncate(working) > 0)
      {
        ret[index--] = (byte)(((Decimal.Truncate(working / 10) % 10) * 16) + (working % 10));
        working /= 100;
      }
      return ret;
    }

    public static decimal BcdToDecimal(byte[] bcd)
    {
      return BcdToDecimal(bcd, false);
    }

    public static decimal BcdToDecimal(byte[] bcd, bool signed)
    {
      return BcdToDecimal(bcd, 0, bcd.Length, signed);
    }

    public static decimal BcdToDecimal(byte[] bcd, int start, int length, bool signed)
    {
      #region Validation
      if (bcd == null)
        throw new ArgumentNullException("bcd");
      #endregion

      int init = (start + length - 1);
      bool negative = false;

      if (signed)
      {
        negative = (bcd[init] & 0x01) != 0;
        init -= 1;
      }

      decimal ret = 0;
      for (int i = init; i >= start; i--)
        ret += (decimal)((((bcd[i] >> 4) * 10) + bcd[i] % 16) * Math.Pow(10, 2 * (((start + length - 1) - i))));

      if (signed && negative)
        ret = (-(ret));

      return ret;
    }
    #endregion
  }
}
