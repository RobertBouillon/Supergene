using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System;

/// <remarks>Usage: using static System.FluentTry;</remarks>
public class FluentTry
{
  public static StatefulTry<T> Try<T>(Func<T> func) => new StatefulTry<T>(func);
  public static FluentTry Try(Action action) => new FluentTry(action);

  public class StatefulTry<T> : FluentTry
  {
    public T Value { get; }
    public StatefulTry(Func<T> action) : base()
    {
      try
      {
        Value = action();
      }
      catch (Exception ex)
      {
        _error = ex;
      }
    }

    public new StatefulTry<T> Catch()
    {
      base.Catch();
      return this;
    }

    public new StatefulTry<T> Catch(Action<Exception> action)
    {
      base.Catch(action);
      return this;
    }

    public new StatefulTry<T> Catch<T1>(Action<T1> action) where T1 : Exception
    {
      base.Catch(action);
      return this;
    }
    public new StatefulTry<T> Finally(Action action)
    {
      base.Finally(action);
      return this;
    }
    public new StatefulTry<T> Catch<T1>(Func<T1, Exception> action) where T1 : Exception
    {
      base.Catch(action);
      return this;
    }
    public new StatefulTry<T> Wrap(Func<Exception, Exception> action)
    {
      base.Wrap(action);
      return this;
    }
    public new bool Succeeded(out T value)
    {
      value = Value;
      return base.Succeeded;
    }

    public new bool Succeeded(out T value, out Exception error)
    {
      value = Value;
      error = _error;
      return base.Succeeded;
    }
  }

  private Exception _error;
  private bool _handled = false;
  protected FluentTry() { }
  public FluentTry(Action action)
  {
    try
    {
      action();
    }
    catch (Exception ex)
    {
      _error = ex;
    }
  }

  public FluentTry Catch()
  {
    if (_error != null && !_handled)
      _handled = true;

    return this;
  }

  public FluentTry Catch(Action<Exception> action)
  {
    #region Validation
    if (action == null)
      throw new ArgumentNullException(nameof(action));
    #endregion
    if (_error != null && !_handled)
    {
      action(_error);
      _handled = true;
    }

    return this;
  }

  public FluentTry Catch<T>(Action<T> action) where T : Exception
  {
    #region Validation
    if (action == null)
      throw new ArgumentNullException(nameof(action));
    #endregion
    if (_error != null && !_handled)
      if (_error is T error)
      {
        action(error);
        _handled = true;
      }

    return this;
  }

  public FluentTry Wrap(Func<Exception, Exception> action)
  {
    if (_error != null && !_handled)
      throw action(_error);
    return this;
  }

  public FluentTry Catch<T>(Func<T, Exception> action) where T : Exception
  {
    if (_error != null && !_handled && _error is T error)
      throw action(error);
    return this;
  }

  public FluentTry Finally(Action action)
  {
    action();
    return this;
  }

  public bool Succeeded => _error is null;

  private static void Test()
  {
    new FluentTry(Foo).Catch(); //Swallow error
    new FluentTry(Foo).Wrap(x => new Exception("Wrapped", x)); //Wrap error
    new FluentTry(Foo).Catch(x => Foo(x)); //Handle Error
    new FluentTry(Foo).Catch<IOException>(x => Foo(x)).Catch(x => Foo(x)); //Cascade Error
    new FluentTry(Foo).Finally(Foo);
    FluentTry.Try(Foo).Finally(Foo);
    var val = FluentTry.Try(Bar).Value;
  }

  private static void Foo() { }
  private static void Foo(Exception ex) { }
  private static int Bar() => 1;
}
