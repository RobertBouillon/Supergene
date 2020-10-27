using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
  public static class ICollectionExtensions
  {
    public static void Remove<T>(this ICollection<T> source, Func<T, bool> filter)
    {
      List<T> remove = new List<T>();

      foreach (T item in source)
        if (filter(item))
          remove.Add(item);

      foreach (T item in remove)
        source.Remove(item);
    }

    public static void Remove<T>(this ICollection<T> source, IEnumerable<T> items)
    {
      foreach (T item in items)
        source.Remove(item);
    }

    public static void Replace<T>(this IList<T> source, T oldItem, T newItem)
    {
      var index = source.IndexOf(oldItem);
      source.RemoveAt(index);
      source.Insert(index, newItem);
    }

  }
}
