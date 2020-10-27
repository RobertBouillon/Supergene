using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Linq
{
  public static class IEnumerableExtensions
  {
    class DistinctQualityComparer<T> : EqualityComparer<T>
    {
      private readonly Func<T, T, bool> _comparer;
      private readonly Func<T, int> _hasher;
      public DistinctQualityComparer(Func<T, T, bool> comparer, Func<T, int> hasher)
      {
        _comparer = comparer;
        _hasher = hasher;
      }

      public override bool Equals(T x, T y)
      {
        return _comparer(x, y);
      }

      public override int GetHashCode(T obj)
      {
        return _hasher(obj);
      }
    }

    public static IEnumerable<IEnumerable<T>> PermuteAll<T>(this IEnumerable<T> values) => Enumerable.Range(1, values.Count()).SelectMany(x => values.Permute(x));
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> values) => values.SelectMany(x => Permute(new[] { new[] { x } }, values, values.Count() - 1));
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> values, int permutations) => values.SelectMany(x => Permute(new[] { new[] { x } }, values, permutations - 1));
    private static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<IEnumerable<T>> current, IEnumerable<T> values, int count) => (count == 0) ? current : (count == 1) ? Permute(current, values) : Permute(Permute(current, values), values, --count);
    private static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<IEnumerable<T>> current, IEnumerable<T> values) => current.SelectMany(x => values.Select(y => x.Concat(new[] { y })));

    public static IEnumerable<T> CopyOf<T>(this IEnumerable<T> list) => list.ToList();
    public static int IndexOf<T>(this IEnumerable<T> list, T target)
    {
      int index = 0;

      foreach (T item in list)
        if (item.Equals(target))
          return index;
        else
          index++;

      return -1;
    }

    public static void Assemble<TItem, TValue>(this IEnumerable<TItem> list, Func<TItem, TValue> lookup, Action<TItem, TItem> setter)
    {
      SortedList<TValue, TItem> slist = new SortedList<TValue, TItem>();
      foreach (var item in list)
        slist.Add(lookup(item), item);

      foreach (var item in list)
        setter(item, slist[lookup(item)]);

    }

    public static void Assign<TItem, TValue>(this IEnumerable<TItem> list, IEnumerable<TValue> assignee, Action<TValue, TItem> assignment)
    {
      var e = assignee.GetEnumerator();
      foreach (var item in list)
      {
        e.MoveNext();
        assignment(e.Current, item);
      }
    }

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, T, bool> comparer, Func<T, int> hasher)
    {
      return list.Distinct(new DistinctQualityComparer<T>(comparer, hasher));
    }

    public static IEnumerable<IGrouping<int, T>> GroupBy<T>(this IEnumerable<T> list, int count)
    {
      int index = 0;
      foreach (var chunk in list.GroupBy(x => index++ / count))
        yield return chunk;
    }

    public static IEnumerable<T> Take<T>(this IEnumerable<T> list, int start, int count)
    {
      IEnumerator<T> e = list.GetEnumerator();
      int index = 0;

      while (e.MoveNext())
        if (++index > start)
          if (count-- > 0)
            yield return e.Current;
    }

    private class Grouping<T> : IGrouping<int, T>
    {
      public int Size { get; set; }
      public int Key { get; set; }
      public IEnumerator<T> Enumerator { get; set; }
      public bool End { get; set; }

      public Grouping(int size, int key, IEnumerator<T> enumerator)
      {
        Size = size;
        Key = key;
        Enumerator = enumerator;
      }

      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < Size; i++)
          if (Enumerator.MoveNext())
            yield return Enumerator.Current;
          else
          {
            End = true;
            break;
          }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < Size; i++)
          if (Enumerator.MoveNext())
            yield return Enumerator.Current;
          else
          {
            End = true;
            break;
          }
      }
    }

    public static IEnumerable<IGrouping<int, T>> GroupsOf<T>(this IEnumerable<T> source, int groupSize)
    {
      var enumerator = source.GetEnumerator();
      int index = 0;
      Grouping<T> group;
      do
      {
        yield return group = new Grouping<T>(groupSize, index++, enumerator);
      } while (!group.End);


      //long index = 0;
      ////foreach(var item in source)
      //return source.Select(x => new { index = index++, item = x }).GroupBy(x => (int)(x.index / groupSize), x => x.item);
    }

    //public static IEnumerable<T> CopyOf<T>(this IEnumerable<T> list)
    //{
    //  List<T> copy = new List<T>(list);
    //  foreach (T item in list.ToList())
    //    yield return item;
    //}

    public static IEnumerable<T> Sync<T>(this IEnumerable<T> list, ReaderWriterLockSlim sync)
    {
      List<T> l = null;
      using (new SafeReaderLock(sync))
        l = new List<T>(list.CopyOf());

      foreach (T item in l)
        yield return item;
    }

    public static IEnumerable<T> Sync<T>(this IEnumerable<T> list)
    {
      return Sync<T>(list, list);
    }

    public static IEnumerable<T> Sync<T>(this IEnumerable<T> list, object sync)
    {
      List<T> l = null;
      using (new SafeLock(sync))
        l = new List<T>(list.CopyOf());

      foreach (T item in l)
        yield return item;
    }

    public static CompareResult<T, T> Compare<T>(this IEnumerable<T> left, IEnumerable<T> right)
    {
      return Compare(left, right, (x, y) => x.Equals(y));
    }

    public static CompareResult<T, TRight> Compare<T, TRight>(this IEnumerable<T> left, IEnumerable<TRight> right, Func<T, TRight, bool> isEqual)
    {
      return Compare(left, right, isEqual, isEqual);
    }

    public static CompareResult<TLeft, TRight> Compare<TLeft, TRight>(this IEnumerable<TLeft> leftList, IEnumerable<TRight> rightList, Func<TLeft, TRight, bool> isEqual, Func<TLeft, TRight, bool> isSame)
    {

      CompareResult<TLeft, TRight> results = new CompareResult<TLeft, TRight>();

      results.Removed.AddRange(leftList.Where(x => rightList.Count(y => isSame(x, y)) == 0));
      results.Added.AddRange(rightList.Where(x => leftList.Count(y => isSame(y, x)) == 0));

      foreach (TLeft left in leftList)
      {
        TRight right = rightList.FirstOrDefault(x => isSame(left, x));

        if (right == null)
          continue;

        if (!isEqual(left, right))
          results.Different.Add(left, right);
        else
          results.Equal.Add(left, right);
      }
      return results;
    }


    //MUST BE PRE-SORTED
    public static IEnumerable<KeyValuePair<TLeft, TRight>> JoinByNestedLoop<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, int> comparer)
    {
      var eleft = left.GetEnumerator();
      var eright = right.GetEnumerator();

      if (!eleft.MoveNext() || !eright.MoveNext())
        yield break;

      //Nested Loop Join
      do
      {
        var comp = comparer(eleft.Current, eright.Current);
        if (comp == 0)
        {
          yield return new KeyValuePair<TLeft, TRight>(eleft.Current, eright.Current);
          if (!eleft.MoveNext())
            break;
        }
        else if (comp > 0)
        {
          if (!eright.MoveNext())
            break;
        }
        else if (comp < 0)
        {
          if (!eleft.MoveNext())
            break;
        }
      } while (true);
    }

    //MUST BE PRE-SORTED
    public static IEnumerable<KeyValuePair<TLeft, TRight>> JoinLeftByNestedLoop<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, int> comparer)
    {
      var eleft = left.GetEnumerator();
      var eright = right.GetEnumerator();

      if (!eleft.MoveNext() || !eright.MoveNext())
        yield break;

      //Nested Loop Join
      do
      {
        var comp = comparer(eleft.Current, eright.Current);
        if (comp == 0)
        {
          yield return new KeyValuePair<TLeft, TRight>(eleft.Current, eright.Current);
          if (!eleft.MoveNext())
            break;
        }
        else if (comp > 0)
        {
          if (!eright.MoveNext())
            break;
        }
        else if (comp < 0)
        {
          yield return new KeyValuePair<TLeft, TRight>(eleft.Current, default(TRight));
          if (!eleft.MoveNext())
            break;
        }
      } while (true);
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
    {
      foreach (var item in source)
        yield return item;
      foreach (var item in items)
        yield return item;
    }

    public static IEnumerable<KeyValuePair<TLeft, TRight>> JoinLeftByHashtable<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, Func<TLeft, TKey> leftSelector, IEnumerable<TRight> right, Func<TRight, TKey> rightSelector)
    {
      //Bigger set is left.
      Dictionary<TKey, TRight> hashtable = new Dictionary<TKey, TRight>();
      hashtable.Load(right, rightSelector);

      foreach (var item in left)
      {
        TRight ritem;
        if (hashtable.TryGetValue(leftSelector(item), out ritem))
          yield return new KeyValuePair<TLeft, TRight>(item, ritem);
      }
    }


    public static IEnumerable<TLeft> FilterByHashtable<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, Func<TLeft, TKey> leftSelector, IEnumerable<TRight> right, Func<TRight, TKey> rightSelector)
    {
      //Bigger set is left.

      HashSet<TKey> hashtable = new HashSet<TKey>();
      foreach (var item in right)
        hashtable.Add(rightSelector(item));

      foreach (var item in left)
        if (hashtable.Contains(leftSelector(item)))
          yield return item;
    }

    /// <summary>
    /// Traverses a single-dimension hierarchy (Walks down children)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public static IEnumerable<T> Traverse<T>(this T obj, Func<T, T> child)
    {
      for (T cursor = obj; cursor != null; cursor = child(cursor))
        yield return cursor;

      //T cursor = obj;
      //do
      //{
      //  yield return cursor;
      //  cursor = child(cursor);
      //} while (cursor != null);
    }

    //?? Use / Test this?
    //public static IEnumerable<T> Traverse<T>(this IEnumerable<T> obj, Func<T, IEnumerable<T>> child) where T : class
    //{
    //  foreach (var item in obj)
    //  {
    //    yield return item;
    //    foreach (var c in item.Traverse(child))
    //      yield return c;
    //  }
    //}

    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> obj, Func<IEnumerable<T>, IEnumerable<T>> child)
    {
      IEnumerable<T> cursor = obj;
      List<T> ret = new List<T>();
      do
      {
        ret.AddRange(cursor);
        cursor = child(cursor);
      } while (cursor != null);

      return ret;
    }

    public static IEnumerable<T> Traverse<T>(this T obj, Func<T, IEnumerable<T>> child, bool inclusive = true)
    {
      List<T> ret = new List<T>();
      if (inclusive)
        ret.Add(obj);

      foreach (T item in child(obj))
        ret.AddRange(item.Traverse(child, true));

      return ret;
    }

    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> root, Func<T, IEnumerable<T>> child, Action<IEnumerable<T>> action, bool inclusive = true)
    {
      List<T> ret = new List<T>();
      Traverse(root, ret, new Stack<T>(), child, action, inclusive);
      return ret;
    }

    private static void Traverse<T>(this IEnumerable<T> root, List<T> list, Stack<T> stack, Func<T, IEnumerable<T>> child, Action<IEnumerable<T>> action, bool inclusive = true)
    {
      if (inclusive)
        list.AddRange(root);

      foreach (T o in root)
      {
        stack.Push(o);
        action(stack);
        child(o).Traverse(list, stack, child, action);
        stack.Pop();
      }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) => Shuffle(items, new Random());
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, int seed) => Shuffle(items, new Random(seed));
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random) => items.OrderBy(x => random.Next());

    public static IEnumerable<T> InclusiveScan<T>(this IEnumerable<T> source, Func<T, T, T> operation, T seed = default)
    {
      foreach (var item in source)
      {
        seed = operation(seed, item);
        yield return seed;
      }
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
      foreach (var item in items)
        action(item);
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
    {
      int i = 0;
      foreach (var item in items)
        action(item, i++);
    }
    //This is already optimized
    //Optimize - only allocate the data needed (specified in count). Bounds already checked by indexer set.
    //public static T[] ToArray<T>(this IEnumerable<T> source, int count)
    //{
    //  T[] ret = new T[count];

    //  int index = 0;
    //  foreach(T item in source)
    //    ret[index++] = item;
    //  return ret;
    //}
  }
}
