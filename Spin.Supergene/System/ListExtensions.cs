using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class ListExtensions
  {
    public static List<IList<T>> Split<T>(this IList<T> o, int size)
    {
      int chunknumber = o.Count / size;
      int lastsize = o.Count & size;
      List<IList<T>> ret = new List<IList<T>>();

      foreach(IGrouping<int,T> group in o.GroupBy(x=>o.IndexOf(x)/size))
        ret.Add(new List<T>(group));

      return ret;
    }
  }
}
