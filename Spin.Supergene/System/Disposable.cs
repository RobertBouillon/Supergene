namespace System;

public abstract class Disposable : IDisposable
{
  public bool IsDisposed => Disposer.IsDisposed;
  protected Disposer Disposer { get; } = new Disposer();

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!Disposer.TryDispose())
      return;

    if (disposing)
      DisposeManaged();
    DisposeNative();
  }

  protected virtual void DisposeManaged() { }
  protected virtual void DisposeNative() { } //NOTE: When implementing DisposeNative, add a finalizer that calls Dispose(false)
}
