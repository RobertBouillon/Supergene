using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Workers
{
  public abstract class EventWorker : Worker
  {
    protected abstract WaitHandle Handle { get; }
    public EventWorker() { }
    public EventWorker(string name) : base(name) { }
    protected override void WaitForWork() => Handle.WaitOne(WaitDelay);
  }
}
