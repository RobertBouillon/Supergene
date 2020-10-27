using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
  public class Try
  {
    public static StatefulTry<T> Do<T>(Func<T> func) => new StatefulTry<T>(func);
    public static Try Do(Action action) => new Try(action);

    public class StatefulTry<T> : Try
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
    }

    private Exception _error;
    private bool _handled = false;
    protected Try() { }
    public Try(Action action)
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

    public Try Catch()
    {
      if (_error != null && !_handled)
        _handled = true;

      return this;
    }

    public Try Catch(Action<Exception> action)
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

    public Try Catch<T>(Action<T> action) where T : Exception
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

    public Try Wrap(Func<Exception, Exception> action)
    {
      if (_error != null && !_handled)
        throw action(_error);
      return this;
    }

    public Try Catch<T>(Func<T, Exception> action) where T : Exception
    {
      if (_error != null && !_handled && _error is T error)
        throw action(error);
      return this;
    }

    public Try Finally(Action action)
    {
      action();
      return this;
    }

    private static void Test()
    {
      new Try(Foo).Catch(); //Swallow error
      new Try(Foo).Wrap(x => new Exception("Wrapped", x)); //Wrap error
      new Try(Foo).Catch(x => Foo(x)); //Handle Error
      new Try(Foo).Catch<IOException>(x => Foo(x)).Catch(x => Foo(x)); //Cascade Error
      new Try(Foo).Finally(Foo);
      Try.Do(Foo).Finally(Foo);
      var val = Try.Do(Bar).Value;
    }

    private static void Foo() { }
    private static void Foo(Exception ex) { }
    private static int Bar() => 1;
  }
}
