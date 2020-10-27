using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
  public static class CollectionTExtensions
  {
    public static void Remove<T>(this IList<T> src, Func<T, bool> condition)
    {
      for (int i = 0; i < src.Count; i++)
        if (condition(src[i]))
          src.RemoveAt(i--);
    }

    public static void Remove<T>(this IList<T> src, IEnumerable<T> toremove)
    {
      foreach (var item in toremove)
        src.Remove(item);
    }
  }
}
