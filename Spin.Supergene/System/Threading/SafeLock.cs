using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace System.Threading;

public class SafeLock : IDisposable
{
  private bool _isLocked;
  private object _target;

  public SafeLock(object target) : this(target, TimeSpan.FromMinutes(1))
  {

  }

  public SafeLock(object target, TimeSpan timeout)
  {
    _target = target;
    if (!Monitor.TryEnter(target, timeout))
    {
      StackTrace trace = new StackTrace();
      Trace.TraceError("A deadlock occurred in {0}. \r\n{1}", trace.GetFrame(2).GetMethod().Name, trace.ToString());
      _isLocked = false;
    }
    else
    {
      _isLocked = true;
    }

  }

  public void Dispose()
  {
    if (_isLocked)
      //Monitor.Pulse(_target);
      Monitor.Exit(_target);
  }
}
