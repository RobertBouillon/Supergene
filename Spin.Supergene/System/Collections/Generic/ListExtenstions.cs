using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic;

public static class ListExtenstions
{
  public static void Sort<T>(this List<T> list, Func<T, IComparable> selector) => list.Sort(new Comparison<T>((x, y) => selector(x).CompareTo(selector(y))));
}
