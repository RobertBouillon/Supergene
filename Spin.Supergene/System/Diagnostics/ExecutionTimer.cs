using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Diagnostics
{
  public static class ExecutionTimer
  {
    public static TimeSpan TimeExecution(EmptyMethod method)
    {
      DateTime started = DateTime.Now;
      method();
      return DateTime.Now - started;
    }
  }
}
