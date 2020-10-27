using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public class Disposer
  {
    public bool IsDisposed { get; private set; }
    public Disposer(){}
    public void Assert()
    {
      if (IsDisposed)
        throw new Exception("Object has been disposed");
    }

    //Helpful for short-hand:  private void Foo() => _disposer.Assert(()=>_disposable.Bar());
    public void Assert(Action action)
    {
      if (IsDisposed)
        throw new Exception("Object has been disposed");
      action();
    }

    //Helpful for short-hand:  private bool Foo() => _disposer.Assert(()=>_disposable.Bar());
    public T Assert<T>(Func<T> func)
    {
      if (IsDisposed)
        throw new Exception("Object has been disposed");
      return func();
    }

    public static void Dispose(params IDisposable[] disposable)
    {
      foreach (var item in disposable)
        item?.Dispose();
    }

    public static void Dispose(IEnumerable<IDisposable> disposable)
    {
      foreach (var item in disposable)
        item?.Dispose();
    }

    public void SetDisposed()
    {
      if (IsDisposed)
        throw new InvalidOperationException("Already disposed");
      IsDisposed = true;
    }

    public bool TryDispose()
    {
      if (IsDisposed)
        return false;
      SetDisposed();
      return true;
    }
  }
}
