using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System;

public static class ArrayExtensions
{
  public static bool IsEqualTo(this Array a, Array b) => (a.Length == b.Length) && !Enumerable.Range(0, a.Length).Any(i => !a.GetValue(i).Equals(b.GetValue(i)));

  //NOTE: This is a reinterpret cast, NOT a conversion.
  public static Array Cast<TFrom, TTo>(this Array source) where TTo : struct where TFrom : struct => MemoryMarshal.Cast<TFrom, TTo>((TFrom[])source).ToArray();
  public static TTo[] Cast<TFrom, TTo>(this TFrom[] source) where TTo : struct where TFrom : struct => MemoryMarshal.Cast<TFrom, TTo>(source).ToArray();

  public static bool Equals(this byte[] a, byte[] b)
  {
    if (a == null || b == null)
      return false;

    var alen = a.Length;
    var blen = b.Length;
    if (alen != blen)
      return false;

    for (int i = 0; i < alen; i++)
      if (a[i] != b[i])
        return false;

    return true;
  }

  public unsafe static bool FastEquals(this byte[] a, byte[] b)
  {
    if (a == null || b == null)
      return false;

    var alen = a.Length;
    var blen = b.Length;
    if (alen != blen)
      return false;

    fixed (byte* ap = &a[0])
    fixed (byte* bp = &b[0])
    {
      byte* x = ap, y = bp;
      for (int i = 0; i < alen; i++)
        if (*(x++) != *(y++))
          return false;
    }

    return true;
  }
}
