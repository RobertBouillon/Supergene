using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic;

public static class QueueTExtensions
{
  public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> range)
  {
    foreach(var o in range)
      queue.Enqueue(o);
  }

  public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int count)
  {
    for (int i = 0; i < count; i++)
      yield return queue.Dequeue();
  }

  public static IEnumerable<T> DequeueUpTo<T>(this Queue<T> queue, int count)
  {
    for (int i = 0; i < count; i++)
      if(queue.Count > 0)
        yield return queue.Dequeue();
  }
}
