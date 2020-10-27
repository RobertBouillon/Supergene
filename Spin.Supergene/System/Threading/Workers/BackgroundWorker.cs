using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Workers
{
  // No idea what this is for. OUT IT GOES.
  public class BackgroundWorker : Worker
  {
    #region Fields
    private Action<WaitHandle, Func<bool>> _loop;
    private ManualResetEvent _stopHandle;
    #endregion

    #region Constructors
    public BackgroundWorker(string name, Action<WaitHandle, Func<bool>> loop) : base(name)
    {
      _loop = loop;
      _stopHandle = new ManualResetEvent(false);
    }
    #endregion

    #region Overrides

    protected override void CancelWork()
    {
    }

    protected override void Work()
    {
      _loop(_stopHandle, () => this.IsStopping);
    }

    protected override void OnStarting(ComponentModel.CancelEventArgs e)
    {
      _stopHandle.Reset();
      base.OnStarting(e);
    }

    protected override void OnStopping(ComponentModel.CancelEventArgs e)
    {
      _stopHandle.Set();
      base.OnStopping(e);
    }
    #endregion
  }
}
