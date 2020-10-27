using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
  public static class ObjectExtensions
  {
    //public static IEnumerable<T> Traverse<T>(this IEnumerable<T> root, Func<T, IEnumerable<T>> func)
    //{
    //  List<T> ret = new List<T>();
    //  foreach (T o in root)
    //    ret.AddRange(func(o));

    //  return ret;
    //}

    //public static void Traverse<T>(this IEnumerable<T> root, Func<T, IEnumerable<T>> func, Action<T, T> action)
    //{
    //  IEnumerable<T> cursor;

    //  foreach (T o in root)
    //    foreach (T kvp in (cursor = func(o)))
    //      action(o, kvp);
    //}

    //public static IDictionary<TSource, TContext> Traverse<TSource,TContext>(this IEnumerable<TSource> root, Func<TSource, IEnumerable<TSource>> func, TContext initContext, Func<TContext, TSource, TContext> setContext)
    //{
    //  Dictionary<TSource, TContext> ret = new Dictionary<TSource, TContext>();

    //  foreach (TSource o in root)
    //    foreach(var kvp in func(o).Traverse(func, setContext(initContext, o), setContext))
    //      ret.Add(kvp.Key, kvp.Value);

    //  return ret;
    //}

    //public static IDictionary<T, T3> Recurse<T, T2, T3>(this IEnumerable<T2> root, Func<T2, T3, IDictionary<T, T3>> func, Func<T2, T3> context)
    //{
    //  Dictionary<T, T3> ret = new Dictionary<T, T3>();

    //  IDictionary<T, T3> cursor;

    //  foreach (T2 o in root)
    //    foreach (var kvp in (cursor = func(o, context(o))))
    //      ret.Add(kvp.Key, kvp.Value);

    //  return ret;
    //}
  }
}
