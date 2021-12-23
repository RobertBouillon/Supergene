using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static class IDisposableExtensions
  {
    public static async Task DisposeAfterAsync<TIn>(this TIn disposable, Func<TIn, Task> action) where TIn : IDisposable
    {
      using(disposable)
        await action(disposable);
    }

    public static async Task<TOut> DisposeAfterAsync<TIn, TOut>(this TIn disposable, Func<TIn, Task<TOut>> func) where TIn : IDisposable
    {
      using(disposable)
        return await func(disposable);
    }

    public static void DisposeAfter<TIn>(this TIn disposable, Action<TIn> action) where TIn : IDisposable
    {
      using (disposable)
        action(disposable);
    }

    public static TOut DisposeAfter<TIn, TOut>(this TIn disposable, Func<TIn, TOut> func) where TIn : IDisposable
    {
      using (disposable)
        return func(disposable);
    }

    public static void DisposeAfter<T>(this IEnumerable<T> disposable, Action<IEnumerable<T>> action) where T : IDisposable
    {
      try
      {
        action(disposable);
      }
      finally
      {
        foreach (var item in disposable)
          item.Dispose();
      }
    }

    public static TOut DisposeAfter<TIn, TOut>(this IEnumerable<TIn> disposable, Func<IEnumerable<TIn>, TOut> action) where TIn : IDisposable
    {
      try
      {
        return action(disposable);
      }
      finally
      {
        foreach (var item in disposable)
          item.Dispose();
      }
    }
  }
}
