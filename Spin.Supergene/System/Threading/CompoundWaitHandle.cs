using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading
{
  public class CompoundWaitHandle : WaitHandle
  {
    #region Fields
    private readonly WaitHandle[] _handles;
    #endregion

    #region Constructors
    public CompoundWaitHandle(params WaitHandle[] handles)
    {
      _handles = handles;
    }
    #endregion

    #region Overrides
    public override bool WaitOne()
    {
      WaitHandle.WaitAll(_handles);
      return base.WaitOne();
    }

    public override bool WaitOne(int millisecondsTimeout, bool exitContext)
    {
      return WaitHandle.WaitAll(_handles, millisecondsTimeout,false);
    }

    public override bool WaitOne(TimeSpan timeout, bool exitContext)
    {
      if (timeout == TimeSpan.Zero)
        timeout = new TimeSpan(0, 0, 0, 0, -1);

      return WaitHandle.WaitAll(_handles, timeout, false);
    }
    #endregion
  }
}
