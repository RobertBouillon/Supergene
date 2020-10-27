using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics
{
  public static class StopwatchExtensions
  {
    public static TimeSpan Time(this Stopwatch sw, Action action)
    {
      sw.Reset();
      sw.Start();
      action();
      sw.Stop();
      return sw.Elapsed;
    }
  }
}
