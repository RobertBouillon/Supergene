using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace System.Threading;

public class SafeReaderLock : IDisposable
{
  private bool _isLocked;
  private ReaderWriterLockSlim _target;

  public SafeReaderLock(ReaderWriterLockSlim target)
    : this(target, TimeSpan.FromMinutes(1))
  {

  }

  public SafeReaderLock(ReaderWriterLockSlim target, TimeSpan timeout)
  {
    _target = target;
    if (!target.TryEnterReadLock(timeout))
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
      _target.ExitReadLock();
  }
}
