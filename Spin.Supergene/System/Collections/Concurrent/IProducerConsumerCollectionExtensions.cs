using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Concurrent;

public static class IProducerConsumerCollectionExtensions
{
  public static IEnumerable<T> TakeAll<T>(this IProducerConsumerCollection<T> source)
  {
    while (source.TryTake(out T item))
      yield return item;
  }
}
