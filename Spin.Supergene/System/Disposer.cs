using System.Collections.Generic;

namespace System;

public class Disposer
{
  private class DisposableEnumeration : IDisposable
  {
    private IEnumerable<IDisposable> _source;
    public DisposableEnumeration(IEnumerable<IDisposable> source) => _source = source;
    public void Dispose() => Disposer.Dispose(_source);
  }

  #region Static Members
  public static IDisposable Wrap(IEnumerable<IDisposable> source) => new DisposableEnumeration(source);

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
  #endregion

  private volatile bool _isDisposed;

  public bool IsDisposed => _isDisposed;

  public Disposer() => _isDisposed = false;

  public void Assert()
  {
    if (IsDisposed)
      throw new Exception("Object has been disposed");
  }

  public T Assert<T>(T field)
  {
    if (IsDisposed)
      throw new Exception("Object has been disposed");
    return field;
  }

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

  public void SetDisposed()
  {
    if (IsDisposed)
      throw new InvalidOperationException("Already disposed");
    _isDisposed = true;
  }

  public bool TryDispose()
  {
    if (IsDisposed)
      return false;
    _isDisposed = true;
    return true;
  }
}
