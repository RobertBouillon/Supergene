using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic;

public static class DictionaryExtensions
{
  public static void Load<TKey, TValue>(this Dictionary<TKey, TValue> target, IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
  {
    foreach (var item in source)
      target.Add(keySelector(item), item);
  }

  public static void Load<TKey, TValue>(this Dictionary<TKey, TValue> target, IEnumerable<TKey> source, Func<TKey, TValue> keySelector)
  {
    foreach (var item in source)
      target.Add(item, keySelector(item));
  }

  public static void Load<TKey, TValue, TSource>(this Dictionary<TKey, TValue> target, IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
  {
    foreach (var item in source)
      target.Add(keySelector(item), valueSelector(item));
  }

  public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> factory = null)
  {
    if (factory == null)
      factory = () => Activator.CreateInstance<TValue>();

    TValue ret;
    if (!dict.TryGetValue(key, out ret))
      dict[key] = ret = factory();
    return ret;
  }
}
