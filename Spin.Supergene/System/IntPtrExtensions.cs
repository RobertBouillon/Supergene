using System;

namespace System;
public static class IntPtrExtensions
{
  public static IntPtr Add(this IntPtr pointer, long offset) => new IntPtr(pointer.ToInt64() + offset);
  public static IntPtr Add(this IntPtr pointer, int offset) => new IntPtr(pointer.ToInt64() + offset);
}
