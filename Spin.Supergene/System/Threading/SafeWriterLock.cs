using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace System.Threading
{
  public class SafeWriterLock : IDisposable
  {
    private bool _isLocked;
    private ReaderWriterLockSlim _target;

    public SafeWriterLock(ReaderWriterLockSlim target)
      : this(target, TimeSpan.FromMinutes(1))
    {

    }

    public SafeWriterLock(ReaderWriterLockSlim target, TimeSpan timeout)
    {
      _target = target;
      if (!target.TryEnterWriteLock(timeout))
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
      {
        //Monitor.Pulse(_target);
        _target.ExitWriteLock();
      }
    }
  }
}
