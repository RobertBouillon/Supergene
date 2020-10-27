using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
  public static class EnumerableEx
  {
    private class GenericEnumerable<T> : IEnumerable<T>
    {
      public Func<T, T> Next { get; }
      public Func<T, bool> HasNext { get; }
      public T Seed { get; }

      public GenericEnumerable(T seed, Func<T, T> next, Func<T, bool> hasNext)
      {
        Next = next;
        HasNext = hasNext;
        Seed = seed;
      }

      public IEnumerator<T> GetEnumerator() => new GenericEnumerator<T>(Seed, Next, HasNext);
      IEnumerator IEnumerable.GetEnumerator() => new GenericEnumerator<T>(Seed, Next, HasNext);
    }

    private class GenericEnumerator<T> : IEnumerator<T>
    {
      public T Current { get; private set; }
      object IEnumerator.Current => Current;
      public Func<T, T> Next { get; }
      public Func<T, bool> HasNext { get; }
      public T Seed { get; }
      private bool _initialized = false;

      public GenericEnumerator(T seed, Func<T, T> next, Func<T, bool> hasNext)
      {
        Next = next;
        HasNext = hasNext;
        Current = Seed = seed;
      }

      public void Dispose() { }

      public bool MoveNext()
      {
        if (!_initialized)
        {
          Current = Seed;
          _initialized = true;
          return true;
        }

        if (HasNext(Current))
        {
          Current = Next(Current);
          return true;
        }
        else
          return false;
      }

      public void Reset() => _initialized = false;
    }

    public static IEnumerable<T> Set<T>(params T[] items) => items;
    public static IEnumerable<T> Iterate<T>(T seed, Func<T, T> next, Func<T, bool> hasNext)
    {
      #region Validation
      if (next is null)
        throw new ArgumentNullException(nameof(next));
      if (hasNext is null)
        throw new ArgumentNullException(nameof(hasNext));
      #endregion

      return new GenericEnumerable<T>(seed, next, hasNext);
    }

    public static IEnumerable<T> Single<T>(T item)
    {
      yield return item;
    }
  }
}
